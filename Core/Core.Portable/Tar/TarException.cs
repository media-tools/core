using System;

namespace Core.Tar
{
	public class TarException : Exception
	{
		public TarException (string message) : base (message)
		{
		}
	}
}