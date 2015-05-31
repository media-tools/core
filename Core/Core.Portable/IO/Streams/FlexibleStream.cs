using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.Common;

namespace Core.IO.Streams
{
	public sealed class FlexibleStream : IFlexibleOutputStream
	{
		AsyncAction<string> OnWrite = OnWrite_NoHandlerWarning;
		AsyncAction OnClose = OnClose_NoHandlerWarning;

		#region IFlexibleOutputStream implementation

		public async Task WriteAsync (string str)
		{
			await OnWrite (str).ConfigureAwait (false);
		}

		public async Task TryClose ()
		{
			await OnClose ();
		}

		#endregion

		public void PipeTo (StreamWriter streamWriter, bool dispose)
		{
			PipeTo (new FlexibleStreamWriter (streamWriter: streamWriter) { IsDisposable = dispose });
		}

		public void PipeTo (IFlexibleOutputStream target)
		{
			// set the write handler
			OnWrite = target.WriteAsync;
			OnClose = target.TryClose;
		}

		public void PipeToLimbo ()
		{
			OnWrite = OnWrite_NoHandlerWarning;
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
	}
}

