using System;

namespace Core.Shell.Common.FileSystems
{
	public abstract class VirtualFile : VirtualNode
	{
		protected VirtualFile (Path path)
			: base (path: path)
		{
		}

		public abstract VirtualFileReader OpenReader ();

		public abstract VirtualFileWriter OpenWriter ();
	}
}

