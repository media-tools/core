using System;
using Core.IO;
using Core.Shell.Common.FileSystems;

namespace Core.Shell.Platform.FileSystems
{
	public abstract class RegularDirectory : VirtualDirectory
	{
		protected RegularDirectory (Path path)
			: base (path: path)
		{
		}

		public override bool Validate (bool throwExceptions)
		{
			bool result = false;
			try {
				if (FileHelper.Instance.IsDirectory (path: Path.RealPath)) {
					result = true;
				} else {
					throw new VirtualIOException (message: "No such directory", node: Path);
				}
			} catch (Exception ex) {
				throw new VirtualIOException (message: ex.Message, node: Path, innerException: ex);
			}
			return result;
		}

		public override VirtualDirectoryListing OpenList ()
		{
			return new RegularDirectoryListing (directory: this);
		}

		public void CreateDirectories ()
		{
			try {
				System.IO.Directory.CreateDirectory (Path.RealPath);
			} catch (Exception ex) {
				throw new VirtualIOException (message: ex.Message, node: Path, innerException: ex);
			}
		}
	}
}

