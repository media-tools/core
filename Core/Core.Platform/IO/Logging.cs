//
// Logging.cs
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
using Core.Common;

namespace Core.IO
{
	public static class Logging
	{
		private static NonBlockingFile logfile;

		public static void Enable ()
		{
			Log.LogHandler += (type, messageLines) => {
				foreach (string message in messageLines)
					Console.WriteLine (message);
				if (Targets.StandardOutput) {
					foreach (string message in messageLines) {
						NonBlockingConsole.WriteLine ("[" + type + "] " + message);
					}
				}
			};

			Log.LogHandler += (type, messageLines) => {
				if (Targets.DefaultLogFile) {
					if (logfile == null) {
						logfile = new NonBlockingFile (Storage.DefaultLogFile);
					}
					foreach (string message in messageLines) {
						logfile.WriteLine (DateTime.Now.ToString ("yyyyMMdd-HHmmss") + " [" + type + "] " + message);
					}
				}
			};

			/*if (type == Log.Type.FATAL_ERROR) {
					MessageBox.Show (message, "Fatal Error");
					Application.Exit ();
				} else {*/
		}

		public static void Finish ()
		{
			NonBlockingConsole.Finish ();
			if (logfile != null) {
				logfile.Finish ();
			}
		}

		public static class Targets
		{
			public static bool StandardOutput { get; set; } = true;

			public static bool DefaultLogFile { get; set; } = true;
		}
	}
}

