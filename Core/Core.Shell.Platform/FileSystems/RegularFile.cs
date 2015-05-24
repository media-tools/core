using System;
using Core.Shell.Common.FileSystems;
using Core.IO;

namespace Core.Shell.Platform.FileSystems
{
	public abstract class RegularFile : VirtualFile
	{
		public string RealPath { get; private set; }

		protected readonly RegularFileSystem fileSystem;

		protected RegularFile (string prefix, string path, RegularFileSystem fileSystem)
			: base (prefix: prefix, path: path)
		{
			this.fileSystem = fileSystem;
			RealPath = prefix + path;
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

