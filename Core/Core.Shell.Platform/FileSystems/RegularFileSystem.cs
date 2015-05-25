using System;
using Core.Shell.Common.FileSystems;
using Core.Common;
using Core.IO;
using Core.Portable;

namespace Core.Shell.Platform.FileSystems
{
	public abstract class RegularFileSystem : FileSystemSubsystem
	{
		readonly Prefix homePrefix;

		protected RegularFileSystem ()
		{
			homePrefix = new Prefix ("~/", this);
			AddPrefix (homePrefix);
		}

		public override string GetRealPath (Prefix prefix, string[] virtualPath)
		{
			string realPath;
			if (prefix == homePrefix) {
				realPath = PathHelper.CombinePath (PlatformInfo.User.HomeDirectory, virtualPath);
			} else {
				realPath = prefix.CombinePath (virtualPath);
			}
			return realPath;
		}

		protected override VirtualNode NodeInternal (Path path)
		{
			if (FileHelper.Instance.IsDirectory (path: path.RealPath)) {
				return Directory (path);
			} else {
				return File (path);
			}
		}

		public override VirtualNode ParseNativePath (string nativePath)
		{
			if (nativePath != null) {
				foreach (Prefix prefix in Prefixes) {
					if (prefix.Matches (nativePath)) {
						return Node (prefix.CreatePath (nativePath));
					}
				}
			}

			return null;
		}
	}
}

