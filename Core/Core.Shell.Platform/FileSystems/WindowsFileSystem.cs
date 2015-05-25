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

		protected override IVirtualFile File (string prefix, string path)
		{
			return new WindowsFile (prefix: prefix, path: path, fileSystem: this);
		}

		protected override IVirtualDirectory Directory (string prefix, string path)
		{
			return new WindowsDirectory (prefix: prefix, path: path, fileSystem: this);
		}

		protected override IVirtualLink Link (string prefix, string path)
		{
			return new WindowsLink (prefix: prefix, path: path, fileSystem: this);
		}
	}
}

