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
		public static string WriteString (params File[] files)
		{
			return ReadableEncoding.Encode (Write (files));
		}

		public static byte[] Write (params File[] files)
		{
			MemoryStream outStream = new MemoryStream ();
			using (var tar = new TarWriter (outStream)) {
				foreach (File file in files) {
					MemoryStream inStream = new MemoryStream (file.BinaryContent);
					tar.Write (inStream, file.BinaryContent.Length, file.Name);
				}
			}
			byte[] binaryContent = outStream.ToArray ();
			return binaryContent;
		}

		public static File[] ReadString (string content)
		{
			byte[] binaryContent = ReadableEncoding.Decode (content);
			return Read (binaryContent);
		}

		public static File[] Read (byte[] binaryContent)
		{
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

