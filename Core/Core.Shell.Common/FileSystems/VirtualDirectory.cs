using System;

namespace Core.Shell.Common.FileSystems
{
	public abstract class VirtualDirectory : VirtualNode
	{
		protected VirtualDirectory (string prefix, string path)
			: base (prefix: prefix, path: path)
		{
		}

		public abstract VirtualDirectoryListing OpenList ();
	}
}
