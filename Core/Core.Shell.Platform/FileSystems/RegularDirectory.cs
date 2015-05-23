using System;
using Core.Shell.Common.FileSystems;

namespace Core.Shell.Platform.FileSystems
{
	public class RegularDirectory : VirtualDirectory
	{
		public string RealPath { get; private set; }

		public override string VirtualPrefix { get { return virtualPrefix; } }

		public override string VirtualPath { get { return virtualPath; } }

		readonly string virtualPrefix;
		readonly string virtualPath;

		public RegularDirectory (string prefix, string path)
		{
			virtualPrefix = prefix;
			virtualPath = path;
			RealPath = prefix + path;
		}

		public override VirtualDirectoryListing OpenList ()
		{
			return new RegularDirectoryListing (directory: this);
		}

		public VirtualNode GetChildDirectory (string name)
		{
			string childPath = $"{virtualPath}/{name}";
			return new RegularDirectory (virtualPrefix, childPath);
		}

		public VirtualNode GetChildFile (string name)
		{
			string childPath = $"{virtualPath}/{name}";
			return new RegularFile (virtualPrefix, childPath);
		}
	}
}

