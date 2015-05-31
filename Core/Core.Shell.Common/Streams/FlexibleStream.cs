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
		readonly List<string> cache = new List<string> ();
		readonly object _lock = new object ();
		bool InCacheMode = false;

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
			Log.Debug ("FlexibleStream.NoHandlerWarning: no output handler assigned! str: ", str);

			return TaskHelper.Completed;
		}

		public void PipeTo (FlexibleStream otherStream)
		{
			PipeTo (async str => await otherStream.WriteAsync (str));
		}

		public void PipeTo (StreamWriter writer)
		{
			PipeTo (async str => await writer.WriteAsync (str));
		}

		public void PipeTo (AsyncAction<string> action)
		{
			lock (_lock) {
				// set the write handler
				OnWrite = async str => await action (str);
				InCacheMode = false;
				// print the cache in the new handler!
				foreach (string str in cache) {
					OnWrite (str);
				}
				cache.Clear ();
			}
		}

		public void PipeToCache ()
		{
			lock (_lock) {
				OnWrite = async str => cache.Add (str);
				InCacheMode = true;
			}
		}

		public void PipeToLimbo ()
		{
			InCacheMode = false;
			OnWrite = NoHandlerWarning;
		}

		public bool TryReadLine (out string result)
		{
			if (InCacheMode && cache.Count > 0) {
				result = string.Empty;
				int i = 0;
				while (cache.Count > 0) {
					if (cache [i].Contains ("\n")) {
						int index = cache [i].IndexOf ("\n");
						result += cache [i].Substring (0, index + 1);
						if (index + 1 < cache [i].Length) {
							cache [i] = cache [i].Substring (index + 1);
						}
						break;
					} else {
						result += cache [i];
						i++;
					}
				}
				cache.RemoveRange (0, i + 1);
				return !string.IsNullOrWhiteSpace (result);
			} else {
				result = null;
				return false;
			}
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

