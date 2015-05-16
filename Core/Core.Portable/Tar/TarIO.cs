using System;
using System.IO;
using System.Text;
using System.Net;
using Core.Common;
using System.Collections.Generic;
using System.Linq;

namespace Core.Tar
{
	public static class TarIO
	{
		public static string Write (params File[] files)
		{
			MemoryStream outStream = new MemoryStream ();
			using (var tar = new TarWriter (outStream)) {
				foreach (File file in files) {
					MemoryStream inStream = new MemoryStream (file.BinaryContent);
					tar.Write (inStream, file.BinaryContent.Length, file.Name);
				}
			}
			byte[] binaryContent = outStream.ToArray ();
			string content = StringEncoding.Encode (binaryContent);
			return content;
		}

		public static File[] Read (string content)
		{
			byte[] binaryContent = StringEncoding.Decode (content);

			List<File> result = new List<File> ();
			MemoryStream inStream = new MemoryStream (binaryContent);
			var tar = new TarReader (inStream);

			while (tar.MoveNext (false)) {
				string path = tar.FileInfo.FileName;
				Log.Debug ("path: ", path);
				MemoryStream outStream = new MemoryStream ();
				tar.Read (outStream);
				File file = File.FromByteArray (name: path, content: outStream.ToArray ());
				result.Add (file);
			}

			return result.ToArray ();
		}

		public static class StringEncoding
		{
			private static readonly HashSet<char> AllowedCharactersChars;
			private static readonly HashSet<byte> AllowedCharactersBytes;

			static StringEncoding ()
			{
				AllowedCharactersChars = new HashSet<char> {
					' ', '_', '-'
				};
				for (char c = 'a'; c <= 'z'; ++c)
					AllowedCharactersChars.Add (c);
				for (char c = 'A'; c <= 'Z'; ++c)
					AllowedCharactersChars.Add (c);
				for (char c = '0'; c <= '9'; ++c)
					AllowedCharactersChars.Add (c);

				AllowedCharactersBytes = new HashSet<byte> (AllowedCharactersChars.Select (c => Convert.ToByte (c)));
			}

			const char escapeSingle = '%';
			const char escapeMulti = '#';
			const int maxLineLength = 100;

			public static string Encode (byte[] bytes)
			{
				//byte[] encodedBytes = WebUtility.UrlEncodeToBytes (bytes, 0, bytes.Length);
				//Log.Debug ("bytes: ", bytes.Length, ", encodedBytes: ", encodedBytes.Length);
				//char[] characters = Encoding.UTF8.GetChars (encodedBytes);
				//string rawString = Encoding.UTF8.GetString (encodedContent, 0, encodedContent.Length);

				int lineLength = 0;
				StringBuilder builder = new StringBuilder ();

				for (int i = 0; i < bytes.Length;) {
					byte b = bytes [i++];

					if (AllowedCharactersBytes.Contains (b)) {
						builder.Append (Convert.ToChar (b));
						lineLength += 1;

					} else if (i < bytes.Length && bytes [i] == b) {

						int times = 1;
						while (i < bytes.Length && bytes [i] == b) {
							i++;
							times++;
						}

						string timesStr = string.Format ("{0}{1:X2}{{{2}}}", escapeMulti, b, times);

						if (times > 100) {
							lineLength = 0;
							builder.Append ('\n');
							builder.Append (timesStr);
							builder.Append ('\n');
						} else {
							builder.Append (timesStr);
							lineLength += timesStr.Length;
						}

					} else {
						builder.Append ($"{escapeSingle}{b:X2}");
						lineLength += 3;
					}

					if (lineLength >= maxLineLength) {
						lineLength = 0;
						builder.Append ('\n');
					}
				}

				return builder.ToString ();
			}

			public static byte[] Decode (string s)
			{
				return Decode (s.ToCharArray ());
			}

			public static byte[] Decode (char[] characters)
			{
				List<byte> bytes = new List<byte> ();

				for (int i = 0; i < characters.Length;) {
					char c = characters [i++];

					if (AllowedCharactersChars.Contains (c)) {
						bytes.Add (Convert.ToByte (c));

					} else if (c == '%' && i + 1 < characters.Length) {
						char n1 = characters [i++];
						char n2 = characters [i++];
						byte b = Convert.ToByte ($"{n1}{n2}", 16);
						bytes.Add (b);

					} else if (c == '#' && i + 3 < characters.Length) {
						char n1 = characters [i++];
						char n2 = characters [i++];
						byte b = Convert.ToByte ($"{n1}{n2}", 16);

						char e1 = characters [i++];
						if (e1 == '{') {
							string timesStr = "";
							while (i + 1 < characters.Length) {
								char t = characters [i];
								if (Char.IsDigit (t)) {
									timesStr += t;
									i++;
								} else {
									break;
								}
							}
							int times;
							if (int.TryParse (s: timesStr, result: out times)) {
								char e2 = characters [i++];
								if (e2 == '}') {
									for (int t = 0; t < times; ++t) {
										bytes.Add (b);
									}
								} else {
									throw new ArgumentException ($"TarIO.StringEncoding.Decode: Invalid character: '{e2}', expected: '}}'");
								}
							} else {
								throw new ArgumentException ($"TarIO.StringEncoding.Decode: Invalid integer: '{timesStr}'");
							}
						} else {
							throw new ArgumentException ($"TarIO.StringEncoding.Decode: Invalid character: '{e1}', expected: '{{'");
						}
					}
				}

				return bytes.ToArray ();
			}
		}

		public class File
		{
			public string Name { get; private set; }

			public byte[] BinaryContent { get; private set; }

			private File ()
			{
			}

			public static File FromByteArray (string name, byte[] content)
			{
				return new File {
					Name = name,
					BinaryContent = content,
				};
			}

			public static File FromString (string name, string content)
			{
				return new File {
					Name = name,
					BinaryContent = Encoding.UTF8.GetBytes (content),
				};
			}
		}
	}
}

