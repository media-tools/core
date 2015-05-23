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

		protected override VirtualFile File (string prefix, string path)
		{
			return new LinuxFile (prefix: prefix, path: path, fileSystem: this);
		}

		protected override VirtualDirectory Directory (string prefix, string path)
		{
			return new LinuxDirectory (prefix: prefix, path: path, fileSystem: this);
		}
	}
}

