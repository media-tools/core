using System;

namespace Core.Shell.Common.FileSystems
{
	public abstract class VirtualDirectory : VirtualNode
	{
		protected VirtualDirectory (Path path)
			: base (path: path)
		{
		}

		public abstract VirtualDirectoryListing OpenList ();
	}
}
