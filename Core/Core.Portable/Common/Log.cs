//
// Log.cs
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
using System.Linq;
using System.Collections.Generic;

namespace Core.Common
{
	public static class Log
	{
		public static Action<Type, IEnumerable<string>> LogHandler = (t, s) => {
		};

		public static int Indent { get; set; }

		public static void Debug (params object[] messages)
		{
			output (Type.DEBUG, messages);
		}

		public static void Info (params object[] messages)
		{
			output (Type.INFO, messages);
		}

		public static void Warning (params object[] messages)
		{
			output (Type.WARNING, messages);
		}

		public static void _Test (params object[] messages)
		{
			output (Type._TEST, messages);
		}

		public static void Toast (params object[] messages)
		{
			output (Type.TOAST, messages);
		}

		public static void Trace (params object[] messages)
		{
			output (Type.TRACE, messages);
		}

		public static void Error (params object[] messages)
		{
			output (Type.ERROR, messages);
		}

		public static void FatalError (params object[] messages)
		{
			output (Type.FATAL_ERROR, messages);
		}

		private static void output (Type type, params object[] messages)
		{
			string rawMessage = string.Join ("", messages.Select (o => o ?? "(null)")).Replace ("\r", "");

			IEnumerable<string> lines = from msg in rawMessage.Split ('\n')
			                            select IndentString + msg;
			LogHandler (type, lines);
		}

		private readonly static int IndentWidth = 4;

		private static string IndentString { get { return String.Concat (Enumerable.Repeat (" ", (int)Indent * IndentWidth)); } }

		public enum Type
		{
			TRACE,
			DEBUG,
			INFO,
			WARNING,
			ERROR,
			FATAL_ERROR,
			TOAST,
			_TEST,
		}
	}
}

