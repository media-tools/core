using System;

namespace Core.Shell.Common.FileSystems
{
	public abstract class VirtualFile : VirtualNode
	{
		protected VirtualFile (string prefix, string path)
			: base (prefix: prefix, path: path)
		{
		}

		public abstract VirtualFileReader OpenReader ();

		public abstract VirtualFileWriter OpenWriter ();
	}
}

