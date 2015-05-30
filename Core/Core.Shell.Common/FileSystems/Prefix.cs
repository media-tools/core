using System;
using System.Linq;
using Core.Common;
using Core.IO;

namespace Core.Shell.Common.FileSystems
{
	public sealed class Prefix
	{
		public string Name { get; private set; }

		public FileSystemSubsystem FileSystem { get; private set; }

		public Prefix (string name, FileSystemSubsystem fileSystem)
		{
			//if (name.ToCharArray ().Any (c => c != '/')) {
			//	name = name.TrimEnd ('/');
			//}
			Name = name;

			FileSystem = fileSystem;
		}

		public bool Matches (string path)
		{
			return path == Name || path == Name.TrimEnd ('/') || path.StartsWith (Name, StringComparison.OrdinalIgnoreCase);
		}

		private string[] GetVirtualPath (string path)
		{
			Log.Debug ("Prefix.GetVirtualPath: Name=", Name, ", path=", path);
			if (path == Name || path == Name.TrimEnd ('/')) {
				return new string[0];
			} else if (path.StartsWith (Name, StringComparison.OrdinalIgnoreCase)) {
				return path.Substring (Name.Length).Split (new []{ '/' }, StringSplitOptions.RemoveEmptyEntries);
			} else {
				throw new VirtualIOException ("Prefix: Cannot create a virtual path array from a string that doesn't match the prefix!", path);
			}
		}

		public Path CreatePath (string path)
		{
			if (Matches (path)) {
				Log.Debug ("Prefix.CreatePath: Name=", Name, ", path=", path, ", result=", new Path (prefix: this, virtualPath: GetVirtualPath (path: path), fileSystem: FileSystem));
				return new Path (prefix: this, virtualPath: GetVirtualPath (path: path), fileSystem: FileSystem);
			} else {
				throw new VirtualIOException ("Prefix: Cannot create a path from a string that doesn't match the prefix!", path);
			}
		}

		public string CombinePath (string[] virtualPath)
		{
			return Name + PathHelper.CombinePath (virtualPath);
		}

		public override bool Equals (object obj)
		{
			if (obj == null)
				return false;
			Prefix other = obj as Prefix;
			return other != null && Name == other.Name;
		}

		public override int GetHashCode ()
		{
			return Name.GetHashCode ();
		}

		public override string ToString ()
		{
			return string.Format ("[Prefix: Name={0}]", Name);
		}
	}
}

