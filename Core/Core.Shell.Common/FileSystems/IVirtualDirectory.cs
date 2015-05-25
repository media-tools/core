using System;

namespace Core.Shell.Common.FileSystems
{
	public interface IVirtualDirectory : IVirtualNode
	{
		VirtualDirectoryListing OpenList ();
	}
}
