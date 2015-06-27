using System;
using Core.IO;

namespace Core.Shell.Common.FileSystems
{
	public abstract class VirtualDirectory : VirtualNode
	{
		protected VirtualDirectory (Path path)
			: base (path: path)
		{
		}

		public VirtualDirectory GetChildDirectory (string name)
		{
			Path childPath = Path.CombinePath (name);
			return Path.FileSystem.Directory (childPath);
		}

		public VirtualFile GetChildFile (string name)
		{
			Path childPath = Path.CombinePath (name);
			return Path.FileSystem.File (childPath);
		}

		public abstract VirtualDirectoryListing OpenList ();
	}
}
