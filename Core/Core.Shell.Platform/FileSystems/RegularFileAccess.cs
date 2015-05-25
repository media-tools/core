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
			return File.ReadLines (file.Path.RealPath);
		}

		public string ReadText ()
		{
			return File.ReadAllText (file.Path.RealPath);
		}

		public byte[] ReadBytes ()
		{
			return File.ReadAllBytes (file.Path.RealPath);
		}

		#endregion

		#region FileWriter implementation

		public void WriteLines (System.Collections.Generic.IEnumerable<string> lines)
		{
			File.WriteAllLines (file.Path.RealPath, lines, Encoding.UTF8);
		}

		public void WriteText (string text)
		{
			File.WriteAllText (file.Path.RealPath, text, Encoding.UTF8);
		}

		public void WriteBytes (byte[] bytes)
		{
			File.WriteAllBytes (file.Path.RealPath, bytes);
		}

		public void AppendLines (System.Collections.Generic.IEnumerable<string> lines)
		{
			File.AppendAllLines (file.Path.RealPath, lines, Encoding.UTF8);
		}

		public void AppendText (string text)
		{
			File.AppendAllText (file.Path.RealPath, text, Encoding.UTF8);
		}

		public void Delete ()
		{
			File.Delete (file.Path.RealPath);
		}

		#endregion
	}

}

