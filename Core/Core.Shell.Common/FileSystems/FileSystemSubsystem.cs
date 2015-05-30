using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common;
using Core.IO;

namespace Core.Shell.Common.FileSystems
{
	public abstract class FileSystemSubsystem
	{
		public Prefix[] Prefixes { get; protected set; } = new Prefix[0];

		public VirtualDirectory DefaultRootDirectory { get; protected set; }

		protected void AddPrefix (params Prefix[] prefix)
		{
			Prefixes = Prefixes.Extend (prefix);
		}

		public VirtualFile File (Path path)
		{
			return getElement (path: path, internalFunc: FileInternal);
		}

		public VirtualDirectory Directory (Path path)
		{
			return getElement (path: path, internalFunc: DirectoryInternal);
		}

		public VirtualLink Link (Path path)
		{
			return getElement (path: path, internalFunc: LinkInternal);
		}

		public VirtualNode Node (Path path)
		{
			return getElement (path: path, internalFunc: NodeInternal);
		}

		T getElement<T> (Path path, Func<Path,T> internalFunc) where T : class
		{
			foreach (Prefix prefix in Prefixes) {
				if (prefix == path.Prefix) {
					try {
						return internalFunc (path);
					} catch (Exception ex) {
						throw new VirtualIOException (message: ex.Message, node: path, innerException: ex);
					}
				}
			}
			return null;
		}

		protected abstract VirtualFile FileInternal (Path path);

		protected abstract VirtualDirectory DirectoryInternal (Path path);

		protected abstract VirtualLink LinkInternal (Path path);

		protected abstract VirtualNode NodeInternal (Path path);

		public abstract VirtualNode ParseNativePath (string nativePath);

		public abstract string GetRealPath (Prefix prefix, string[] virtualPath);
	}

}

