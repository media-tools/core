//
// NonBlockingConsole.cs
//
// Author:
//       Tobias Schulz <tobiasschulz.code@outlook.de>
//
// Copyright (c) 2015 Tobias Schulz
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Common;

namespace Core.IO
{
	public interface IReadLine
	{
		CancellationToken CancelToken { get; set; }

		string Line { get; }

		SpecialCommands SpecialCommand { get; }

		bool IsOpen { get; }

		Task<bool> IsOpenAsync { get; }

		bool TryReadLine ();

		Task<bool> TryReadLineAsync ();
	}

	public interface IHistoryReadLine : IReadLine
	{
		InputHistory History { get; set; }
	}

	public enum SpecialCommands
	{
		None = 0,
		CloseStream = 1,
		/*ArrowUp,
		ArrowDown,
		ArrowLeft,
		ArrowRight,*/
	}

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

