using System;
using Core.Shell.Common.FileSystems;

namespace Core.Shell.Platform.FileSystems
{
	public sealed class WindowsFile : RegularFile
	{
		public WindowsFile (Path path)
			: base (path)
		{
		}

		public override string PermissionsString { get { return ""; } }

		public override string OwnerName { get { return Core.IO.FileHelper.Instance.GetOwnerName (path: Path.RealPath); } }

		public override string GroupName { get { return Core.IO.FileHelper.Instance.GetGroupName (path: Path.RealPath); } }
	}

	public sealed class WindowsDirectory : RegularDirectory
	{
		public WindowsDirectory (Path path)
			: base (path)
		{
		}

		public override string PermissionsString { get { return ""; } }

		public override string OwnerName { get { return Core.IO.FileHelper.Instance.GetOwnerName (path: Path.RealPath); } }

		public override string GroupName { get { return Core.IO.FileHelper.Instance.GetGroupName (path: Path.RealPath); } }
	}

	public sealed class WindowsLink : RegularLink
	{
		public WindowsLink (Path path)
			: base (path)
		{
		}

		public override string PermissionsString { get { return ""; } }

		public override string OwnerName { get { return Core.IO.FileHelper.Instance.GetOwnerName (path: Path.RealPath); } }

		public override string GroupName { get { return Core.IO.FileHelper.Instance.GetGroupName (path: Path.RealPath); } }
	}
}
