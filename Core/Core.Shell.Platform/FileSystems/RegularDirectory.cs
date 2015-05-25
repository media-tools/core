using System;
using Core.IO;
using Core.Shell.Common.FileSystems;

namespace Core.Shell.Platform.FileSystems
{
	public abstract class RegularDirectory : RegularNode, IVirtualDirectory
	{
		protected RegularDirectory (string prefix, string path, RegularFileSystem fileSystem)
			: base (prefix: prefix, path: path, fileSystem: fileSystem)
		{
		}

		public override bool Validate (bool throwExceptions)
		{
			bool result = false;
			try {
				if (FileHelper.Instance.IsDirectory (path: RealPath)) {
					result = true;
				} else {
					throw new VirtualIOException (message: "No such directory", node: this);
				}
			} catch (Exception ex) {
				throw new VirtualIOException (message: ex.Message, node: this, innerException: ex);
			}
			return result;
		}

		public VirtualDirectoryListing OpenList ()
		{
			return new RegularDirectoryListing (directory: this);
		}

		public IVirtualNode GetChildDirectory (string name)
		{
			string childPath = FileSystemHelper.CombinePath (preserveFrontSlash: false, parts: new[]{ VirtualPath, name });
			return fileSystem.Directory (VirtualPrefix + childPath);
		}

		public IVirtualNode GetChildFile (string name)
		{
			string childPath = FileSystemHelper.CombinePath (preserveFrontSlash: false, parts: new[]{ VirtualPath, name });
			return fileSystem.File (VirtualPrefix + childPath);
		}
	}
}

