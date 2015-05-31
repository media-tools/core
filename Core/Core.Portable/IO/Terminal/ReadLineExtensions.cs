using System;

namespace Core.IO.Terminal
{
	public static class ReadLineExtensions
	{
		public static IDisposable UseHistory (this IHistoryReadLine readLine, InputHistory history)
		{
			return new ReadLineHistoryChanger (readLine: readLine, history: history);
		}

		private class ReadLineHistoryChanger : IDisposable
		{
			readonly IHistoryReadLine readLine;
			readonly InputHistory previousHistory;

			public ReadLineHistoryChanger (IHistoryReadLine readLine, InputHistory history)
			{
				this.readLine = readLine;
				this.previousHistory = readLine.History;
				readLine.History = history;
			}

			#region IDisposable implementation

			public void Dispose ()
			{
				readLine.History = previousHistory;
			}

			#endregion
		}
	}
}
