using System;
using Core.Shell.Common.FileSystems;
using Core.IO;

namespace Core.Shell.Platform.FileSystems
{
	public class LinuxFile : RegularFile
	{
		public LinuxFile (Path path)
			: base (path)
		{
		}

		public override string PermissionsString { get { return FileHelper.Instance.PermissionsString (path: Path.RealPath); } }

		public override string OwnerName { get { return Core.IO.FileHelper.Instance.GetOwnerName (path: Path.RealPath); } }

		public override string GroupName { get { return Core.IO.FileHelper.Instance.GetGroupName (path: Path.RealPath); } }
	}

	public class LinuxDirectory : RegularDirectory
	{
		public LinuxDirectory (Path path, RegularFileSystem fileSystem)
			: base (path)
		{
		}

		public override string PermissionsString { get { return FileHelper.Instance.PermissionsString (path: Path.RealPath); } }

		public override string OwnerName { get { return Core.IO.FileHelper.Instance.GetOwnerName (path: Path.RealPath); } }

		public override string GroupName { get { return Core.IO.FileHelper.Instance.GetGroupName (path: Path.RealPath); } }
	}

	public class LinuxLink : RegularLink
	{
		public LinuxLink (Path path, RegularFileSystem fileSystem)
			: base (path)
		{
		}

		public override string PermissionsString { get { return FileHelper.Instance.PermissionsString (path: Path.RealPath); } }

		public override string OwnerName { get { return Core.IO.FileHelper.Instance.GetOwnerName (path: Path.RealPath); } }

		public override string GroupName { get { return Core.IO.FileHelper.Instance.GetGroupName (path: Path.RealPath); } }
	}
}

