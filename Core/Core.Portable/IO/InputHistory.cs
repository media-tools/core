//
// InputHistory.cs
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
using System.Collections.Generic;
using System.Linq;
using Core.Common;

namespace Core.IO
{
	public class InputHistory
	{
		public List<string> History { get; private set; } = new List<string> { };

		public int Index { get; set; } = 0;

		public string Current {
			get {
				if (Index >= 0 && Index < History.Count)
					return History [Index];
				else
					return string.Empty;
			}
		}

		public void Up ()
		{
			if (Index - 1 >= 0) {
				Index--;
			}
		}

		public void Down ()
		{
			// allow one more than the length of the array!
			if (Index + 1 < History.Count + 1) {
				Index++;
			}
		}

		public void Add (string value)
		{
			if (!string.IsNullOrWhiteSpace (value)) {
				History.Add (value);
			}
			Index = History.Count;
		}
	}
	
}
