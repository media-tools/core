using System;
using Core.Shell.Common.FileSystems;

namespace Core.Shell.Platform.FileSystems
{
	public class WindowsFile : RegularFile
	{
		public WindowsFile (string prefix, string path, RegularFileSystem fileSystem)
			: base (prefix, path, fileSystem)
		{
		}

		public override string PermissionsString { get { return ""; } }
	}

	public class WindowsDirectory : RegularDirectory
	{
		public WindowsDirectory (string prefix, string path, RegularFileSystem fileSystem)
			: base (prefix, path, fileSystem)
		{
		}

		public override string PermissionsString { get { return ""; } }
	}
}
