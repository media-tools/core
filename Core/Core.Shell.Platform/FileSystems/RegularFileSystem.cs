using System;
using Core.Shell.Common.FileSystems;
using Core.Common;

namespace Core.Shell.Platform.FileSystems
{
	public abstract class RegularFileSystem : FileSystemSubsystem
	{
		protected RegularFileSystem ()
		{
		}

		protected override VirtualNode Node (string prefix, string path)
		{
			if (RegularFileUtilities.IsDirectory (realPath: prefix + path)) {
				return Directory (prefix, path);
			} else {
				return File (prefix, path);
			}
		}

		public override VirtualNode ParseNativePath (string nativePath)
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

