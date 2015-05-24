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
using System.Threading;
using System.Text;

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

		public static bool TryReadKey (out ConsoleKeyInfo result)
		{
			while (IsInputOpen) {
				if (queueInput.TryTake (out result, 50)) {
					return true;
				}
			}
			result = default(ConsoleKeyInfo);
			return false;
		}

		public static bool TryReadLine (out string result, out SpecialCommand specialCommand)
		{
			specialCommand = SpecialCommand.None;

			var buf = new StringBuilder ();
			while (IsInputOpen) {
				ConsoleKeyInfo key;
				if (TryReadKey (result: out key)) {

					Console.WriteLine (key.Key + " " + key.Modifiers);

					// Ctrl-D
					if (key.Key == ConsoleKey.D && key.Modifiers == ConsoleModifiers.Control) {
						result = null;
						IsInputOpen = false;
						return false;
					}
					// F4 ? WTF ?
					else if (key.Key == ConsoleKey.F4) {
						result = null;
						IsInputOpen = false;
						return false;
					}
					// Arrow Keys
					else if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.LeftArrow || key.Key == ConsoleKey.RightArrow) {
						result = null;
						// remove all current chars
						while (buf.Length > 0) {
							buf.Remove (buf.Length - 1, 1);
							System.Console.Write ("\b \b");
						}
						specialCommand =
							  key.Key == ConsoleKey.UpArrow ? SpecialCommand.ArrowUp
							: key.Key == ConsoleKey.DownArrow ? SpecialCommand.ArrowDown
							: key.Key == ConsoleKey.LeftArrow ? SpecialCommand.ArrowLeft
							: key.Key == ConsoleKey.RightArrow ? SpecialCommand.ArrowRight
							: SpecialCommand.None;
						return true;
					}
					// Enter
					else if (key.Key == ConsoleKey.Enter) {
						result = buf.ToString ();
						System.Console.Write (Environment.NewLine);
						return true;
					}
					// Backspace
					else if (key.Key == ConsoleKey.Backspace && buf.Length > 0) {
						buf.Remove (buf.Length - 1, 1);
						System.Console.Write ("\b \b");
					}
					// normal character
					else if (key.KeyChar != 0) {
						buf.Append (key.KeyChar);
						System.Console.Write (key.KeyChar);
					}
				}
			}

			result = buf.ToString ();
			return !string.IsNullOrEmpty (result);
		}
	}

	public enum SpecialCommand
	{
		None = 0,
		ArrowUp,
		ArrowDown,
		ArrowLeft,
		ArrowRight,
	}
}

