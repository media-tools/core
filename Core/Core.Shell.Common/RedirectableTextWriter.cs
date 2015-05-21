using System;
using System.IO;
using Core.Common;
using System.Text;

namespace Core.Shell.Common
{
	public class RedirectableTextWriter : TextWriter
	{
		public Action<string> Stream = s => new ArgumentException ("UnixShell.Output must not be null!").ThrowAction<string> ();

		public override void Write (char value)
		{
			Stream (value + "");
		}

		public override void Write (string value)
		{
			Stream (value);
		}

		public override Encoding Encoding {
			get { return Encoding.UTF8; }
		}
	}
}

