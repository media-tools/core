using System;
using System.IO;

namespace Core.Shell.Common.FileSystems
{
	public abstract class VirtualNode
	{
		public string VirtualPrefix { get; private set; }

		public string VirtualPath { get; private set; }

		public string VirtualFileName { get { return Path.GetFileName (VirtualPath); } }

		public abstract string PermissionsString { get; }

		public abstract string Owner { get; }

		protected VirtualNode (string prefix, string path)
		{
			VirtualPrefix = prefix;
			VirtualPath = path;
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
