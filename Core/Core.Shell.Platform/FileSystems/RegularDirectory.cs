using System;
using Core.Shell.Common.FileSystems;

namespace Core.Shell.Platform.FileSystems
{
	public abstract class RegularDirectory : VirtualDirectory
	{
		public string RealPath { get; private set; }

		public override string VirtualPrefix { get { return prefix; } }

		public override string VirtualPath { get { return path; } }

		private readonly string prefix;
		private readonly string path;
		protected readonly RegularFileSystem fileSystem;

		protected RegularDirectory (string prefix, string path, RegularFileSystem fileSystem)
		{
			this.fileSystem = fileSystem;
			this.prefix = prefix;
			this.path = path;
			RealPath = prefix + path;
		}

		public override VirtualDirectoryListing OpenList ()
		{
			return new RegularDirectoryListing (directory: this);
		}

		public VirtualNode GetChildDirectory (string name)
		{
			string childPath = FileSystemHelper.CombinePath (path, name);
			return fileSystem.Directory (prefix + childPath);
		}

		public VirtualNode GetChildFile (string name)
		{
			string childPath = FileSystemHelper.CombinePath (path, name);
			return fileSystem.File (prefix + childPath);
		}
	}
}

