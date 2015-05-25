using System;

namespace Core.Shell.Common.FileSystems
{
	public class VirtualIOException : Exception
	{
		public VirtualIOException (string message, Path node, Exception innerException)
			: this (message: message, path: node.ToString (), innerException: innerException)
		{
		}

		public VirtualIOException (string message, Path node)
			: this (message: message, path: node.ToString ())
		{
		}

		public VirtualIOException (string message, string path, Exception innerException)
			: base (message: (path != null && !message.Contains (path) ? path + ": " : string.Empty) + message, innerException: innerException)
		{
		}

		public VirtualIOException (string message, string path)
			: base (message: (path != null && !message.Contains (path) ? path + ": " : string.Empty) + message)
		{
		}
	}
}

