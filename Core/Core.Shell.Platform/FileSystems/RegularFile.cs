using System;
using Core.Shell.Common.FileSystems;
using Core.IO;

namespace Core.Shell.Platform.FileSystems
{
	public abstract class RegularFile : VirtualFile
	{
		protected RegularFile (Path path)
			: base (path: path)
		{
		}

		public override bool Validate (bool throwExceptions)
		{
			bool result = false;
			try {
				if (FileHelper.Instance.IsFile (path: Path.RealPath)) {
					result = true;
				} else {
					throw new VirtualIOException (message: "No such file", node: Path);
				}
			} catch (Exception ex) {
				throw new VirtualIOException (message: ex.Message, node: Path, innerException: ex);
			}
			return result;
		}

		public override VirtualFileReader OpenReader ()
		{
			return new RegularFileAccess (file: this);
		}

		public override VirtualFileWriter OpenWriter ()
		{
			return new RegularFileAccess (file: this);
		}
	}
}

