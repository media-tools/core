using System;

namespace Core.Shell.Common
{
	public class UnixShell
	{
		public Action<string> Output = s => {
		};

		public UnixShell ()
		{
		}

		public void Interactive (string line)
		{
			Output (line);
		}
	}
}

