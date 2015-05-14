using System;
using System.Threading;

namespace Core.Portable
{
	public static class Thread
	{
		/*public static void Sleep (TimeSpan timeSpan)
		{
			// Set is never called, so we wait always until the timeout occurs
			using (EventWaitHandle tmpEvent = new ManualResetEvent (false)) {
				tmpEvent.WaitOne (timeSpan);
			}
		}*/

		public static void Sleep (int milliseconds)
		{
			System.Threading.Tasks.Task.Delay (3000).Wait ();
		}
	}
}

