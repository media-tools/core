using System;
using Core.Common;
using Core.IO;

namespace Core.Shell.Common.FileSystems
{
	public sealed class Path
	{
		public Prefix Prefix { get; private set; }

		public string[] VirtualPath { get; private set; }

		public string RealPath { get; set; }

		public string FileName { get { return PathHelper.GetFileName (VirtualPath); } }

		public FileSystemSubsystem FileSystem { get; private set; }

		public Path (Prefix prefix, string[] virtualPath, FileSystemSubsystem fileSystem)
		{
			Prefix = prefix;
			VirtualPath = virtualPath;
			FileSystem = fileSystem;
			RealPath = fileSystem.GetRealPath (prefix, virtualPath);
		}

		public Path CombinePath (params string[] parts)
		{
			return new Path (prefix: Prefix, virtualPath: PathHelper.NormalizePath (VirtualPath.Extend (parts)), fileSystem: FileSystem);
		}

		public string FullPath ()
		{
			return Prefix.CombinePath (VirtualPath);
		}

		public override bool Equals (object obj)
		{
			if (obj == null)
				return false;
			Path other = obj as Path;
			return other != null && Prefix == other.Prefix && VirtualPath.Join ("/") == other.VirtualPath.Join ("/");
		}

		public override int GetHashCode ()
		{
			return ToString ().GetHashCode ();
		}

		public override string ToString ()
		{
			return string.Format ("[Path: Prefix={0}, VirtualPath={1}]", Prefix, VirtualPath.Join ("/"));
		}
	}
}

