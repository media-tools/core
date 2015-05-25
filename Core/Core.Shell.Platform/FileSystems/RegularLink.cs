using System;
using Core.IO;
using Core.Shell.Common.FileSystems;

namespace Core.Shell.Platform.FileSystems
{
	public abstract class RegularLink : VirtualLink
	{
		protected RegularLink (Path path)
			: base (path: path)
		{
		}

		public override bool Validate (bool throwExceptions)
		{
			bool result = false;
			try {
				if (FileHelper.Instance.IsSymLink (path: Path.RealPath)) {
					result = true;
				} else {
					throw new VirtualIOException (message: "No such link", node: Path);
				}
			} catch (Exception ex) {
				throw new VirtualIOException (message: ex.Message, node: Path, innerException: ex);
			}
			return result;
		}
	}
}
