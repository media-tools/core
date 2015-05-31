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
using Core.IO.Streams;

namespace Core.IO
{
	public static class NonBlockingConsole
	{
		private static readonly BlockingCollection<string> queueOutput = new BlockingCollection<string> ();
		private static readonly BlockingCollection<ConsoleKeyInfo> queueInput = new BlockingCollection<ConsoleKeyInfo> ();

		private static bool running;
		private static readonly Thread threadOutput;
		private static readonly Thread threadInput;
		private static readonly object lockObject = new object ();
		private static bool runningOutput;
		private static bool runningInput;

		public static bool IsInputOpen { get; private set; } = false;

		static NonBlockingConsole ()
		{
			// unit tests
			if (Core.Portable.PlatformInfo.System.IsRunningFromNUnit) {
				running = false;
				IsInputOpen = false;
			}
			// normal program
			else {
				running = true;
				threadOutput = new Thread (
					() => {
						runningOutput = true;
						string item;
						while (running) {
							if (queueOutput.TryTake (out item, 50)) {
								lock (lockObject) {
									Console.Write (item);
								}
							}
						}
						runningOutput = false;
					});
				threadOutput.IsBackground = true;
				threadOutput.Start ();

				threadInput = new Thread (
					() => {
						runningInput = true;
						while (running) {
							while (Console.KeyAvailable) {
								lock (lockObject) {
									queueInput.Add (Console.ReadKey (true));
								}
							}
							Thread.Sleep (50);
						}
						runningInput = false;
					});
				threadInput.IsBackground = true;
				threadInput.Start ();

				IsInputOpen = Core.Portable.PlatformInfo.System.IsInteractive;
			}
		}

		public static void Finish ()
		{
			// unit tests
			if (Core.Portable.PlatformInfo.System.IsRunningFromNUnit) {
				running = false;
				IsInputOpen = false;
			}
			// normal program
			else {
				
				running = false;
				for (int i = 10; i >= 0 && (runningInput || runningOutput); i--) {
					Thread.Sleep (30);
				}
				IsInputOpen = false;

				threadOutput.Abort ();
				while (queueOutput.Count > 0) {
					Console.WriteLine (queueOutput.Take ());
				}
				threadInput.Abort ();
			}
		}

		public static void Write (string value)
		{
			queueOutput.Add (value);
		}

		public static void WriteLine (string value)
		{
			queueOutput.Add (value + "\n");
		}

		public static Task WriteAsync (string value)
		{
			Write (value);
			return TaskHelper.Completed;
		}

		public static Task WriteLineAsync (string value)
		{
			WriteLine (value);
			return TaskHelper.Completed;
		}

		public static IFlexibleOutputStream ToFlexibleStream ()
		{
			return new FlexibleNonBlockingConsole ();
		}

		public class FlexibleNonBlockingConsole : IFlexibleOutputStream
		{
			#region IFlexibleStream implementation

			// Analysis disable once MemberHidesStaticFromOuterClass
			async Task IFlexibleOutputStream.WriteAsync (string str)
			{
				await NonBlockingConsole.WriteAsync (str);
			}

			Task IFlexibleOutputStream.TryClose ()
			{
				return Actions.EmptyAsync ();
			}

			#endregion
		}

		public static bool TryReadKey (out ConsoleKeyInfo result, CancellationToken cancelToken)
		{
			while (IsInputOpen) {
				if (cancelToken.IsCancellationRequested) {
					break;
				}
				if (queueInput.TryTake (out result, 50)) {
					return true;
				}
			}
			result = default(ConsoleKeyInfo);
			return false;
		}

		public class ReadLine : IHistoryReadLine
		{
			public CancellationToken CancelToken { get; set; } = CancellationToken.None;

			public bool IsOpen { get { return NonBlockingConsole.IsInputOpen; } }

			public string Line { get; private set; }

			public SpecialCommands SpecialCommand { get; private set; }

			public InputHistory History { get; set; }

			public ReadLine (InputHistory history)
			{
				History = history;
			}

			public bool TryReadLine ()
			{
				// reset state 
				Line = null;
				SpecialCommand = SpecialCommands.None;

				var buf = new StringBuilder ();
				while (NonBlockingConsole.IsInputOpen) {
					if (CancelToken.IsCancellationRequested) {
						break;
					}

					ConsoleKeyInfo key;
					if (NonBlockingConsole.TryReadKey (result: out key, cancelToken: CancelToken)) {

						//Console.WriteLine (key.Key + " " + key.Modifiers);

						//Log.Debug ("Index: ", history.Index, ", History: ", history.History.Select (l => "\"" + l + "\"").Join (", "));

						// Ctrl-D
						if (key.Key == ConsoleKey.D && key.Modifiers == ConsoleModifiers.Control) {
							SpecialCommand = SpecialCommands.CloseStream;
							return true;
						}
						// F4 ? WTF ?
						else if (key.Key == ConsoleKey.F4) {
							SpecialCommand = SpecialCommands.CloseStream;
							return true;
						}
						// Arrow Keys
						else if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.LeftArrow || key.Key == ConsoleKey.RightArrow) {

							// save at current history position
							//history.Edit (buf.ToString ());

							// remove all current chars
							while (buf.Length > 0) {
								buf.Remove (buf.Length - 1, 1);
								System.Console.Write ("\b \b");
							}

							if (History != null) {
								// move inside the history
								if (key.Key == ConsoleKey.UpArrow)
									History.Up ();
								else if (key.Key == ConsoleKey.DownArrow)
									History.Down ();

								// load the current history position
								buf.Append (History.Current);
								System.Console.Write (buf);
							}
						}
						// Enter
						else if (key.Key == ConsoleKey.Enter) {
							if (History != null) {
								History.Add (buf.ToString ());
							}
							Line = buf.ToString ();
							System.Console.WriteLine ();
							return true;
						}
						// Backspace
						else if (key.Key == ConsoleKey.Backspace) {
							if (buf.Length > 0) {
								buf.Remove (buf.Length - 1, 1);
								System.Console.Write ("\b \b");
							}
						}
						// normal character
						else if (key.KeyChar != 0) {
							buf.Append (key.KeyChar);
							System.Console.Write (key.KeyChar);
						}
					}
				}

				Line = buf.ToString ();
				return !string.IsNullOrEmpty (Line);
			}

			public Task<bool> TryReadLineAsync ()
			{
				return Task.FromResult (TryReadLine ());
			}
		}
	}
}

