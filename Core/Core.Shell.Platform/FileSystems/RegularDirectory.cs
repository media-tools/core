using System;
using Core.IO;
using Core.Shell.Common.FileSystems;

namespace Core.Shell.Platform.FileSystems
{
	public abstract class RegularDirectory : VirtualDirectory
	{
		public string RealPath { get; private set; }

		protected readonly RegularFileSystem fileSystem;

		protected RegularDirectory (string prefix, string path, RegularFileSystem fileSystem)
			: base (prefix: prefix, path: path)
		{
			this.fileSystem = fileSystem;
			RealPath = prefix + path;
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

		public override VirtualDirectoryListing OpenList ()
		{
			return new RegularDirectoryListing (directory: this);
		}

		public VirtualNode GetChildDirectory (string name)
		{
			string childPath = FileSystemHelper.CombinePath (VirtualPath, name);
			return fileSystem.Directory (VirtualPrefix + childPath);
		}

		public VirtualNode GetChildFile (string name)
		{
			string childPath = FileSystemHelper.CombinePath (VirtualPath, name);
			return fileSystem.File (VirtualPrefix + childPath);
		}
	}
}

