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
			DefaultRootDirectory = new WindowsDirectory ("c:/", "", this);
		}

		protected override VirtualFile File (string prefix, string path)
		{
			return new WindowsFile (prefix: prefix, path: path, fileSystem: this);
		}

		protected override VirtualDirectory Directory (string prefix, string path)
		{
			return new WindowsDirectory (prefix: prefix, path: path, fileSystem: this);
		}
	}
}

