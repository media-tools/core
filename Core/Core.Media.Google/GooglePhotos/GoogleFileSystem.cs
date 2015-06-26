using System;
using Core.Shell.Common.FileSystems;
using Picasa = Google.Picasa;

namespace Core.Media.Google.GooglePhotos
{
	public sealed class GoogleFileSystem : FileSystemSubsystem
	{
		readonly Prefix prefix;
		public readonly GooglePhotosService Service;

		public GoogleFileSystem (GooglePhotosService service)
		{
			Service = service;

			prefix = new Prefix ("google-photos:/", this);
			AddPrefix (prefix);
		}

		public GoogleFile File (Picasa.Album picasaAlbum, Picasa.Photo picasaPhoto)
		{
			return new GoogleFile (new Path (prefix, new string[]{ picasaAlbum.Title, picasaPhoto.Title }, this));
		}

		#region implemented abstract members of FileSystemSubsystem

		protected override VirtualFile FileInternal (Path path)
		{
			throw new NotImplementedException ();
		}

		protected override VirtualDirectory DirectoryInternal (Path path)
		{
			throw new NotImplementedException ();
		}

		protected override VirtualLink LinkInternal (Path path)
		{
			throw new NotImplementedException ();
		}

		#endregion

		public override string GetRealPath (Prefix prefix, string[] virtualPath)
		{
			string realPath;
			if (prefix == homePrefix) {
				realPath = PathHelper.CombinePath (PlatformInfo.User.HomeDirectory, virtualPath);
			} else {
				realPath = prefix.CombinePath (virtualPath);
			}
			return realPath;
		}

		protected override VirtualNode NodeInternal (Path path)
		{
			if (FileHelper.Instance.IsDirectory (path: path.RealPath)) {
				return Directory (path);
			} else {
				return File (path);
			}
		}

		public override VirtualNode ParseNativePath (string nativePath)
		{
			if (nativePath != null) {
				foreach (Prefix prefix in Prefixes) {
					if (prefix.Matches (nativePath)) {
						return Node (prefix.CreatePath (nativePath));
					}
				}
			}

			return null;
		}

		public static void Register (GooglePhotosService service)
		{
			FileSystemSubsystem fs = new GoogleFileSystem (service);

			if (fs != null) {
				FileSystemSubsystems.Subsystems.Add (fs);
				FileSystemSubsystems.DefaultRootDirectory = fs.DefaultRootDirectory;
			}
		}
	}
}

