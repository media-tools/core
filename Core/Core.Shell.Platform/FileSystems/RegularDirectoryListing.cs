using System;
using Core.Shell.Common.FileSystems;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Core.Common;
using Core.IO;

namespace Core.Shell.Platform.FileSystems
{
	public class RegularDirectoryListing : VirtualDirectoryListing
	{
		readonly RegularDirectory directory;

		public RegularDirectoryListing (RegularDirectory directory)
		{
			this.directory = directory;
		}

		IVirtualNode[] listing = null;

		#region VirtualDirectoryListing implementation

		public IEnumerable<IVirtualFile> ListFiles ()
		{
			if (listing == null) {
				listing = createListing ().ToArray ();
			}
			return listing.OfType<IVirtualFile> ();
		}

		public IEnumerable<IVirtualDirectory> ListDirectories ()
		{
			if (listing == null) {
				listing = createListing ().ToArray ();
			}
			return listing.OfType<IVirtualDirectory> ();
		}

		#endregion

		IEnumerable<IVirtualNode> createListing ()
		{
			Log.Debug ("Directory listing: ", directory);
			Log.Indent++;

			var directories = SafeDirectoryEnumerator.EnumerateDirectories (directory.RealPath, "*", SearchOption.TopDirectoryOnly);
			foreach (string realPath in directories) {
				Log.Debug ("realPath: ", realPath);
				IVirtualNode node = directory.GetChildDirectory (Path.GetFileName (realPath));
				if (node != null) {
					yield return node;
				}
			}
			var files = SafeDirectoryEnumerator.EnumerateFiles (directory.RealPath, "*", SearchOption.TopDirectoryOnly);
			foreach (string realPath in files) {
				Log.Debug ("realPath: ", realPath);
				IVirtualNode node = directory.GetChildFile (Path.GetFileName (realPath));
				if (node != null) {
					yield return node;
				}
			}

			Log.Indent--;
		}
	}
}

