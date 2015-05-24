using System;
using Core.IO;
using Core.Shell.Common.FileSystems;

namespace Core.Shell.Platform.FileSystems
{
	public abstract class RegularLink : VirtualLink
	{
		public string RealPath { get; private set; }

		protected readonly RegularFileSystem fileSystem;

		protected RegularLink (string prefix, string path, RegularFileSystem fileSystem)
			: base (prefix: prefix, path: path)
		{
			this.fileSystem = fileSystem;
			RealPath = prefix + path;
		}

		public override bool Validate (bool throwExceptions)
		{
			bool result = false;
			try {
				if (FileHelper.Instance.IsSymLink (path: RealPath)) {
					result = true;
				} else {
					throw new VirtualIOException (message: "No such file", node: this);
				}
			} catch (Exception ex) {
				throw new VirtualIOException (message: ex.Message, node: this, innerException: ex);
			}
			return result;
		}
	}
}
