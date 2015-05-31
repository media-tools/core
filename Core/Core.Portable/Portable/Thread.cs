using System;
using System.Threading;
using System.Threading.Tasks;

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
			Task.Delay (milliseconds).Wait ();
		}

		public static async Task SleepAsync (int milliseconds)
		{
			await Task.Delay (milliseconds);
		}
	}
}

