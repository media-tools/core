using System;
using System.Collections.Generic;

namespace Core.Shell.Common.FileSystems
{
	public interface VirtualFileWriter
	{
		void WriteLines (IEnumerable<string> lines);

		void WriteText (string text);

		void WriteBytes (byte[] bytes);

		void AppendLines (IEnumerable<string> lines);

		void AppendText (string content);

		void Delete ();
	}
}

