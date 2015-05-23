using System;
using Core.Shell.Common.FileSystems;

namespace Core.Shell.Platform.FileSystems
{
	public class WindowsFileSystem : RegularFileSystem
	{
		public WindowsFileSystem ()
		{
			for (char prefix = 'c'; prefix <= 'z'; prefix++) {
				AddPrefix (prefix + ":/");
			}
			DefaultRootDirectory = new RegularDirectory ("c:/", "");
		}
	}
}

