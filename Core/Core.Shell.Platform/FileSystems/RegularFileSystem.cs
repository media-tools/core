using System;
using Core.Shell.Common.FileSystems;

namespace Core.Shell.Platform.FileSystems
{
	public abstract class RegularFileSystem : FileSystemSubsystem
	{
		protected RegularFileSystem ()
		{
		}

		protected override VirtualFile File (string prefix, string path)
		{
			return new RegularFile (prefix: prefix, path: path);
		}

		protected override VirtualDirectory Directory (string prefix, string path)
		{
			return new RegularDirectory (prefix: prefix, path: path);
		}

		protected override VirtualNode Node (string prefix, string path)
		{
			if (RegularFileUtilities.IsDirectory (realPath: prefix + path)) {
				return new RegularDirectory (prefix, path);
			} else {
				return new RegularFile (prefix, path);
			}
		}

		public override VirtualNode ParseNativePath (string nativePath)
		{
			if (nativePath != null) {
				nativePath = nativePath.Replace ('\\', '/').TrimEnd ('/');

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

