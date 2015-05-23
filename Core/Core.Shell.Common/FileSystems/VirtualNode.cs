using System;
using System.IO;

namespace Core.Shell.Common.FileSystems
{
	public abstract class VirtualNode
	{
		public abstract string VirtualPrefix { get; }

		public abstract string VirtualPath { get; }

		public string VirtualFileName { get { return Path.GetFileName (VirtualPath); } }

		public abstract string PermissionsString { get; }

		protected VirtualNode ()
		{
		}

		public override string ToString ()
		{
			return string.Format ("{0}{1}", VirtualPrefix, VirtualPath);
		}

		public override bool Equals (object obj)
		{
			VirtualNode other = obj as VirtualNode;
			if (other != null) {
				return VirtualPrefix == other.VirtualPrefix && VirtualPath == other.VirtualPath;
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
