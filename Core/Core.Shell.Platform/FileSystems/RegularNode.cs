using System;
using Core.Shell.Common.FileSystems;
using Core.Portable;

namespace Core.Shell.Platform.FileSystems
{
	public abstract class RegularNode : VirtualNode
	{
		public string RealPath { get; private set; }

		protected readonly RegularFileSystem fileSystem;

		protected RegularNode (string prefix, string path, RegularFileSystem fileSystem)
			: base (prefix: prefix, path: path)
		{
			this.fileSystem = fileSystem;
			RealPath = RegularFileSystemHelper.RealPath (prefix: prefix, path: path);
		}

	}
}

