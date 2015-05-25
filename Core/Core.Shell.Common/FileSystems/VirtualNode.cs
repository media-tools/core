using System;
using System.IO;
using Core.IO;

namespace Core.Shell.Common.FileSystems
{
	public abstract class VirtualNode
	{
		public Path Path { get; private set; }

		public abstract string PermissionsString { get; }

		public abstract string OwnerName { get; }

		public abstract string GroupName { get; }

		protected VirtualNode (Path path)
		{
			Path = path;
		}

		public abstract bool Validate (bool throwExceptions);

		public override string ToString ()
		{
			return string.Format ("[{0}: Path={1}]", this.GetType ().Name, Path);
		}

		public override bool Equals (object obj)
		{
			VirtualNode other = obj as VirtualNode;
			if (other != null) {
				return Path == other.Path;
			} else {
				return false;
			}
		}

		public override int GetHashCode ()
		{
			return ToString ().GetHashCode ();
		}
	}
}
