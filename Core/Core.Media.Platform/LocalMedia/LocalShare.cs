using System;
using System.Collections.Generic;
using Core.Media.Common;
using Core.Shell.Common.FileSystems;
using Core.Shell.Platform.FileSystems;
using System.Linq;
using Core.Common;

namespace Core.Media.Platform.LocalMedia
{
	public class LocalShare : Share
	{
		public VirtualDirectory RootDirectory;

		public LocalShare (string rootPath)
		{
			VirtualDirectory d = FileSystemSubsystems.ParseNativePath (nativePath: rootPath) as RegularDirectory;
			if (d == null) {
				throw new ArgumentNullException ("rootPath", "rootPath is no valid directory");
			}
			RootDirectory = d;
		}

		#region implemented abstract members of Share

		public override IEnumerable<Album> GetAlbums ()
		{
			IEnumerable<VirtualDirectory> directories = recursivelyListDirectories (RootDirectory);
			foreach (VirtualDirectory dir in directories) {
				string name = string.Join ("/", dir.Path.VirtualPath.Skip (RootDirectory.Path.VirtualPath.Length));
				yield return new LocalAlbum (name: name, directory: dir);
			}
		}

		#endregion

		IEnumerable<VirtualDirectory> recursivelyListDirectories (VirtualDirectory dir)
		{
			Log.Debug (dir);
			VirtualFile[] files = dir.OpenList ().ListFiles ().ToArray ();
			VirtualDirectory[] subdirs = dir.OpenList ().ListDirectories ().ToArray ();

			if (files.Length > 0) {
				yield return dir;
			}

			if (subdirs.Length > 0) {
				foreach (VirtualDirectory subdir in subdirs) {
					foreach (VirtualDirectory element in recursivelyListDirectories(subdir)) {
						yield return element;
					}
				}
			}
		}
	}
}

