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
	public class FlexibleStream
	{
		protected AsyncAction<string> OnWrite = NoHandlerWarning;

		public async Task WriteAsync (params object[] values)
		{
			foreach (object value in values) {
				//Log.Debug ("FlexibleStream.WriteAsync: ", value);
				await OnWrite (value.ToString ()).ConfigureAwait (false);
			}
		}

		public async Task WriteLineAsync (params object[] values)
		{
			foreach (object value in values) {
				//Log.Debug ("FlexibleStream.WriteAsync: ", value);
				await OnWrite (value.ToString ()).ConfigureAwait (false);
			}
			await OnWrite (Environment.NewLine);
		}

		static Task NoHandlerWarning (string str)
		{
			Log.Debug ("CommandOutputStream.OnWrite: no output handler assigned! str: ", str);

			return TaskHelper.Completed;
		}

		public void PipeTo (FlexibleStream otherStream)
		{
			OnWrite = async str => await otherStream.WriteAsync (str);
		}

		public void PipeTo (AsyncAction<string> action)
		{
			OnWrite = async str => await action (str);
		}

		public async Task Eat (StreamReader reader)
		{
			string line;
			while ((line = await reader.ReadLineAsync ()) != null) {

				Log.Debug ("received: ", line);
				await WriteLineAsync (line);
			}
		}

		public TextWriter ToTextWriter ()
		{
			return new InternalTextWriter (this);
		}

		public class InternalTextWriter : TextWriter
		{
			FlexibleStream flexi;

			public InternalTextWriter (FlexibleStream flexi)
			{
				this.flexi = flexi;
			}

			public override void Write (char value)
			{
				//Log.Debug ("InternalTextWriter.Write: ", value);
				flexi.WriteAsync (value + "").Wait ();
			}

			public override void Write (string value)
			{
				//Log.Debug ("InternalTextWriter.Write: ", value);
				flexi.WriteAsync (value).Wait ();
			}

			public override Encoding Encoding {
				get { return Encoding.UTF8; }
			}
		}
	}
}

