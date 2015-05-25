using System;

namespace Core.Shell.Common.FileSystems
{
	public interface IVirtualFile : IVirtualNode
	{
		VirtualFileReader OpenReader ();

		VirtualFileWriter OpenWriter ();
	}
}

