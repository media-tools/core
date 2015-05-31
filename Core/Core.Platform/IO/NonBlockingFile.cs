//
// NonBlockingFile.cs
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
using System.IO;
using System.Text;
using System.Threading;

namespace Core.IO
{
	public class NonBlockingFile
	{
		private readonly BlockingCollection<string> m_Queue = new BlockingCollection<string> ();

		private bool running;
		private Thread thread;
		private StreamWriter writer;

		public NonBlockingFile (string fileName)
		{
			try {
				Directory.CreateDirectory (Path.GetDirectoryName (fileName));

				writer = new StreamWriter (fileName, true, Encoding.UTF8);

				thread = new Thread (
					() => {
						while (running) {
							writer.WriteLine (m_Queue.Take ());
							writer.Flush ();
						}
					});
				thread.IsBackground = true;
				thread.Start ();
			} catch (IOException ex) {
				Console.WriteLine (ex);
			}
		}

		public void Finish ()
		{
			running = false;
			if (thread != null) {
				thread.Abort ();
			}
			try {
				while (m_Queue.Count > 0) {
					writer.WriteLine (m_Queue.Take ());
					writer.Flush ();
				}
			} catch (IOException ex) {
				Console.WriteLine (ex);
			}
		}

		public void WriteLine (string value)
		{
			m_Queue.Add (value);
		}
	}

}

