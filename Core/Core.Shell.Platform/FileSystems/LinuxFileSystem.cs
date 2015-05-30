using System;
using Core.Shell.Common.FileSystems;

namespace Core.Shell.Platform.FileSystems
{
	public sealed class LinuxFileSystem : RegularFileSystem
	{
		public LinuxFileSystem ()
		{
			Prefix rootPrefix = new Prefix ("/", this);
			AddPrefix (rootPrefix);
			DefaultRootDirectory = new LinuxDirectory (rootPrefix.CreatePath ("/"), this);
		}

		protected override VirtualFile FileInternal (Path path)
		{
			return new LinuxFile (path: path);
		}

		protected override VirtualDirectory DirectoryInternal (Path path)
		{
			return new LinuxDirectory (path: path, fileSystem: this);
		}

		protected override VirtualLink LinkInternal (Path path)
		{
			return new LinuxLink (path: path, fileSystem: this);
		}
	}
}

