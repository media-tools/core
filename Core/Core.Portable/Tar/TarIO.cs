using System;
using System.IO;
using System.Text;
using System.Net;
using Core.Common;

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
			byte[] tarContent = outStream.ToArray ();
			return Encode (tarContent);
		}

		private static string Encode (byte[] bytes)
		{
			byte[] encodedBytes = WebUtility.UrlEncodeToBytes (bytes, 0, bytes.Length);
			Log.Debug ("bytes: ", bytes.Length, ", encodedBytes: ", encodedBytes.Length);
			char[] characters = Encoding.UTF8.GetChars (encodedBytes);

			//string rawString = Encoding.UTF8.GetString (encodedContent, 0, encodedContent.Length);

			const char percent = '%';
			const int maxLineLength = 100;
			int lineLength = 0;
			StringBuilder builder = new StringBuilder ();
			for (int i = 0; i < characters.Length;) {
				char c = characters [i];
				if (c == percent && i + 6 < characters.Length && characters [i + 3] == percent && characters [i + 4] == characters [i + 1] && characters [i + 5] == characters [i + 2]) {
					char n1 = characters [++i];
					char n2 = characters [++i];
					i++;
				
					int times = 1;

					while (i + 2 < characters.Length && characters [i] == percent && characters [i + 1] == n1 && characters [i + 2] == n2) {
						times++;
						i += 3;
					}

					string timesStr = string.Format ("{0}{1}{2}{{{3}}}", percent, n1, n2, times);

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
					builder.Append (c);
					lineLength++;
					i++;
				}

				if (lineLength >= maxLineLength) {
					lineLength = 0;
					builder.Append ('\n');
				}
			}

			return builder.ToString ();
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

