using System;
using Core.Shell.Common.FileSystems;

namespace Core.Shell.Platform.FileSystems
{
	public class WindowsFileSystem : RegularFileSystem
	{
		readonly Prefix cPrefix;

		public WindowsFileSystem ()
		{
			cPrefix = new Prefix ("c:/", this);
			AddPrefix (cPrefix);
			for (char prefix = 'd'; prefix <= 'z'; prefix++) {
				AddPrefix (new Prefix (prefix + ":/", this));
			}
			DefaultRootDirectory = new WindowsDirectory (cPrefix.CreatePath ("c:/"));
		}

		protected override VirtualFile FileInternal (Path path)
		{
			return new WindowsFile (path: path);
		}

		protected override VirtualDirectory DirectoryInternal (Path path)
		{
			return new WindowsDirectory (path: path);
		}

		protected override VirtualLink LinkInternal (Path path)
		{
			return new WindowsLink (path: path);
		}
	}
}

