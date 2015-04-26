using System;
using System.Threading;

namespace Core.Common
{
	public static class PortableThread
	{
		public static void Sleep (TimeSpan timeSpan)
		{
			// Set is never called, so we wait always until the timeout occurs
			using (EventWaitHandle tmpEvent = new ManualResetEvent (false)) {
				tmpEvent.WaitOne (timeSpan);
			}
		}

		public static void Sleep (int milliseconds)
		{
			Sleep (TimeSpan.FromMilliseconds (milliseconds));
		}
	}
}

