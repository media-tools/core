using System;
using Core.Shell.Common.FileSystems;
using Core.Common;
using Core.IO;

namespace Core.Shell.Platform.FileSystems
{
	public abstract class RegularFileSystem : FileSystemSubsystem
	{
		protected RegularFileSystem ()
		{
			AddPrefix ("~/");
		}

		protected override IVirtualNode Node (string prefix, string path)
		{
			if (FileHelper.Instance.IsDirectory (path: RegularFileSystemHelper.RealPath (prefix: prefix, path: path))) {
				return Directory (prefix, path);
			} else {
				return File (prefix, path);
			}
		}

		public override IVirtualNode ParseNativePath (string nativePath)
		{
			if (nativePath != null) {

				foreach (string prefix in Prefixes) {
					if (nativePath.StartsWith (prefix, StringComparison.OrdinalIgnoreCase)) {
						return Node (prefix, nativePath.Substring (prefix.Length));
					}
				}
			}

			return null;
		}
	}
}

