using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Common;
using Core.IO;

namespace Core.IO.Streams
{
	public sealed class FlexibleStream : IFlexibleStream
	{
		readonly List<string> cache = new List<string> ();
		readonly object _lock = new object ();
		bool InCacheMode = false;

		AsyncAction<string> OnWrite = OnWrite_NoHandlerWarning;
		AsyncAction OnClose = OnClose_NoHandlerWarning;

		#region IFlexiblePipeTarget implementation

		public async Task WriteAsync (string str)
		{
			await WriteAsync (new object[]{ str });
		}

		public async Task TryClose ()
		{
			await OnClose ();
		}

		#endregion

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

		/*public void PipeTo (FlexibleStream otherStream)
		{
			PipeTo (
				onWrite: async str => await otherStream.WriteAsync (str),
				onClose: async () => await otherStream.TryClose ()
			);
		}*/

		public void PipeTo (StreamWriter streamWriter, bool dispose)
		{
			PipeTo (new FlexibleStreamWriter (streamWriter: streamWriter) { IsDisposable = dispose });
		}

		public void PipeTo (IFlexibleStream target)
		{
			lock (_lock) {
				// set the write handler
				OnWrite = target.WriteAsync;// async str => await onWrite (str);
				OnClose = target.TryClose;//onClose;
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
				OnWrite = str => {
					cache.Add (str);
					return TaskHelper.Completed;
				};
				OnClose = Actions.EmptyAsync;
				InCacheMode = true;
			}
		}

		public void PipeToLimbo ()
		{
			InCacheMode = false;
			OnWrite = OnWrite_NoHandlerWarning;
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

		public async Task Eat (IReadLine readLine, CancellationToken cancelToken)
		{
			readLine.CancelToken = cancelToken;
			while (readLine.IsOpen && !cancelToken.IsCancellationRequested) {
				if (await readLine.TryReadLineAsync ()) {
					if (readLine.SpecialCommand == SpecialCommands.CloseStream) {
						Log.Debug ("try close!");
						await TryClose ();
						return;
					} else {
						Log.Debug ("eat: ", readLine.Line);
						await WriteLineAsync (readLine.Line);
					}
				}
			}
			readLine.CancelToken = CancellationToken.None;
		}

		static Task OnWrite_NoHandlerWarning (string str)
		{
			Log.Debug ("FlexibleStream.OnWrite_NoHandlerWarning: no output handler assigned! str: ", str);
			return TaskHelper.Completed;
		}

		static Task OnClose_NoHandlerWarning ()
		{
			Log.Debug ("FlexibleStream.OnClose_NoHandlerWarning: no output handler assigned!");
			return TaskHelper.Completed;
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

