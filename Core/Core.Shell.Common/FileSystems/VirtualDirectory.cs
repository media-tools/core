using System;

namespace Core.Shell.Common.FileSystems
{
	public abstract class VirtualDirectory : VirtualNode
	{
		protected VirtualDirectory ()
		{
		}

		public abstract VirtualDirectoryListing OpenList ();
	}
}
