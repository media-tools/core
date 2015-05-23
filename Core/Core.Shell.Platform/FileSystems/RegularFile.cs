using System;
using Core.Shell.Common.FileSystems;

namespace Core.Shell.Platform.FileSystems
{
	public abstract class RegularFile : VirtualFile
	{
		public string RealPath { get; private set; }

		public override string VirtualPrefix { get { return prefix; } }

		public override string VirtualPath { get { return path; } }

		private readonly string prefix;
		private readonly string path;
		protected readonly RegularFileSystem fileSystem;

		protected RegularFile (string prefix, string path, RegularFileSystem fileSystem)
		{
			this.fileSystem = fileSystem;
			this.prefix = prefix;
			this.path = path;
			RealPath = prefix + path;
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

