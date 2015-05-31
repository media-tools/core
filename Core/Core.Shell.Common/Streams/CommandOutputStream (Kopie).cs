using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.Common;
using Core.Portable;
using Core.Shell.Common.FileSystems;
using System.Text;

namespace Core.Shell.Common.Streams
{
	public class CommandOutputStreamKopie
	{
		public AsyncAction<string> OnWrite = NoHandlerWarning;

		public async Task WriteAsync (string value)
		{
			await OnWrite (value);
		}

		public async Task WriteLineAsync (string value)
		{
			await OnWrite (value + Environment.NewLine);
		}

		static Task NoHandlerWarning (string str)
		{
			Log.Debug ("CommandOutputStream.OnWrite: no output handler assigned! str: ", str);

			return TaskHelper.Completed;
		}

		public TextWriter ToTextWriter ()
		{
			return new InternalTextWriter ();
		}

		public class InternalTextWriter : TextWriter
		{
			public override void Write (char value)
			{
				WriteAsync (value + "").Wait ();
			}

			public override void Write (string value)
			{
				WriteAsync (value).Wait ();
			}

			public override Encoding Encoding {
				get { return Encoding.UTF8; }
			}
		}
	}
}

