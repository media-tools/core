using System;
using Core.Shell.Common.FileSystems;
using Core.IO;

namespace Core.Shell.Platform.FileSystems
{
	public abstract class RegularFile : RegularNode, IVirtualFile
	{
		protected RegularFile (string prefix, string path, RegularFileSystem fileSystem)
			: base (prefix: prefix, path: path, fileSystem: fileSystem)
		{
		}

		public override bool Validate (bool throwExceptions)
		{
			bool result = false;
			try {
				if (FileHelper.Instance.IsFile (path: RealPath)) {
					result = true;
				} else {
					throw new VirtualIOException (message: "No such file", node: this);
				}
			} catch (Exception ex) {
				throw new VirtualIOException (message: ex.Message, node: this, innerException: ex);
			}
			return result;
		}

		public VirtualFileReader OpenReader ()
		{
			return new RegularFileAccess (file: this);
		}

		public VirtualFileWriter OpenWriter ()
		{
			return new RegularFileAccess (file: this);
		}
	}
}

