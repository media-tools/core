using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Common
{
	public static class ReadableEncoding
	{
		private static readonly HashSet<char> AllowedCharactersChars;
		private static readonly HashSet<byte> AllowedCharactersBytes;

		static ReadableEncoding ()
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

					if (Convert.ToChar (b) == '\n') {
						lineLength = 0;
						builder.Append ('\n');
					} else {
						lineLength += 3;
					}
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
}

