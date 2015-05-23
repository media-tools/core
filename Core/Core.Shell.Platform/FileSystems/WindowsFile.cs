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

		public override string Owner { get { return Core.IO.FileHelper.Instance.GetOwner (path: RealPath); } }
	}

	public class WindowsDirectory : RegularDirectory
	{
		public WindowsDirectory (string prefix, string path, RegularFileSystem fileSystem)
			: base (prefix, path, fileSystem)
		{
		}

		public override string PermissionsString { get { return ""; } }

		public override string Owner { get { return Core.IO.FileHelper.Instance.GetOwner (path: RealPath); } }
	}

	public class WindowsLink : RegularLink
	{
		public WindowsLink (string prefix, string path, RegularFileSystem fileSystem)
			: base (prefix, path, fileSystem)
		{
		}

		public override string PermissionsString { get { return ""; } }

		public override string Owner { get { return Core.IO.FileHelper.Instance.GetOwner (path: RealPath); } }
	}
}
