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

		protected override VirtualFile File (string prefix, string path)
		{
			return new RegularFile (prefix: prefix, path: path);
		}

		protected override VirtualDirectory Directory (string prefix, string path)
		{
			return new RegularDirectory (prefix: prefix, path: path);
		}
	}
}

