//
// Networking.cs
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
using Newtonsoft.Json;
using System.IO;

namespace Core.IO
{
	public static class ConfigHelper
	{
		public static Config OpenConfig<Config> (string fullPath) where Config : class, new()
		{
			string dirname = Path.GetDirectoryName (fullPath);
			if (!string.IsNullOrEmpty (dirname)) {
				Directory.CreateDirectory (dirname);
			}
			File.AppendAllText (fullPath, "");

			string content = File.ReadAllText (fullPath);
			Config stuff = PortableConfigHelper.ReadConfig<Config> (content: ref content);
			File.WriteAllText (fullPath, content);
			return stuff;
		}

		public static void SaveConfig<Config> (string fullPath, Config stuff) where Config : class, new()
		{
			string dirname = Path.GetDirectoryName (fullPath);
			if (!string.IsNullOrEmpty (dirname)) {
				Directory.CreateDirectory (dirname);
			}
			File.AppendAllText (fullPath, "");

			string content = PortableConfigHelper.WriteConfig (stuff: stuff);
			File.WriteAllText (fullPath, content);
		}
	}
}

