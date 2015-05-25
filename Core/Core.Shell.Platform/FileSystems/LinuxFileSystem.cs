using System;
using Core.Shell.Common.FileSystems;

namespace Core.Shell.Platform.FileSystems
{
	public class LinuxFileSystem : RegularFileSystem
	{
		public LinuxFileSystem ()
		{
			AddPrefix ("/");
			DefaultRootDirectory = new LinuxDirectory ("/", "", this);
		}

		protected override IVirtualFile File (string prefix, string path)
		{
			return new LinuxFile (prefix: prefix, path: path, fileSystem: this);
		}

		protected override IVirtualDirectory Directory (string prefix, string path)
		{
			return new LinuxDirectory (prefix: prefix, path: path, fileSystem: this);
		}

		protected override IVirtualLink Link (string prefix, string path)
		{
			return new LinuxLink (prefix: prefix, path: path, fileSystem: this);
		}
	}
}

