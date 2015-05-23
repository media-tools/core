using System;
using Core.Shell.Common.FileSystems;

namespace Core.Shell.Platform.FileSystems
{
	public class RegularFile : VirtualFile
	{
		public string RealPath { get; private set; }

		public override string VirtualPrefix { get { return virtualPrefix; } }

		public override string VirtualPath { get { return virtualPath; } }

		readonly string virtualPrefix;
		readonly string virtualPath;

		public RegularFile (string prefix, string path)
		{
			virtualPrefix = prefix;
			virtualPath = path;
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

