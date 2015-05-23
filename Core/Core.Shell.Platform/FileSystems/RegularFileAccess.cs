using System;
using Core.Shell.Common.FileSystems;
using System.IO;
using System.Text;

namespace Core.Shell.Platform.FileSystems
{
	public class RegularFileAccess : VirtualFileReader, VirtualFileWriter
	{
		readonly RegularFile file;

		public RegularFileAccess (RegularFile file)
		{
			this.file = file;
		}

		#region FileReader implementation

		public System.Collections.Generic.IEnumerable<string> ReadLines ()
		{
			return File.ReadLines (file.RealPath);
		}

		public string ReadText ()
		{
			return File.ReadAllText (file.RealPath);
		}

		public byte[] ReadBytes ()
		{
			return File.ReadAllBytes (file.RealPath);
		}

		#endregion

		#region FileWriter implementation

		public void WriteLines (System.Collections.Generic.IEnumerable<string> lines)
		{
			File.WriteAllLines (file.RealPath, lines, Encoding.UTF8);
		}

		public void WriteText (string text)
		{
			File.WriteAllText (file.RealPath, text, Encoding.UTF8);
		}

		public void WriteBytes (byte[] bytes)
		{
			File.WriteAllBytes (file.RealPath, bytes);
		}

		public void AppendLines (System.Collections.Generic.IEnumerable<string> lines)
		{
			File.AppendAllLines (file.RealPath, lines, Encoding.UTF8);
		}

		public void AppendText (string text)
		{
			File.AppendAllText (file.RealPath, text, Encoding.UTF8);
		}

		public void Delete ()
		{
			File.Delete (file.RealPath);
		}

		#endregion
	}

}

