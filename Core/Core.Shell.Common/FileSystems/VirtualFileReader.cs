using System;
using System.Collections.Generic;

namespace Core.Shell.Common.FileSystems
{
	public interface VirtualFileReader
	{
		IEnumerable<string> ReadLines ();

		string ReadText ();

		byte[] ReadBytes ();
	}
}

