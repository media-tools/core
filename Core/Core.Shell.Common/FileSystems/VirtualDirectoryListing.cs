using System;
using System.Collections.Generic;

namespace Core.Shell.Common.FileSystems
{
	public interface VirtualDirectoryListing
	{
		IEnumerable<VirtualFile> ListFiles ();

		IEnumerable<VirtualDirectory> ListDirectories ();
	}
}

