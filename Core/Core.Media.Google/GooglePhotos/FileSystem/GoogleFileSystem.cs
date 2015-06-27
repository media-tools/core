using System;
using System.Collections.Generic;
using System.Linq;
using Core.IO;
using Core.Shell.Common.FileSystems;
using Picasa = Google.Picasa;

namespace Core.Media.Google.GooglePhotos.FileSystem
{
	public sealed class GoogleFileSystem : FileSystemSubsystem
	{
		public readonly Dictionary<string, GooglePhotosService> Services = new Dictionary<string, GooglePhotosService> ();

		readonly Prefix mainPrefix;

		public GoogleFileSystem ()
		{
			mainPrefix = new Prefix ("google-photos:/", this);
			AddPrefix (mainPrefix);
		}

		public GoogleFile File (GooglePhotosService service, DiscretePicasaAlbum picasaAlbum, Picasa.Photo picasaPhoto)
		{
			return new GoogleFile (new Path (mainPrefix, new string[] {
				PicasaExtensions.VirtualDirectoryName (service),
				PicasaExtensions.VirtualDirectoryName (picasaAlbum),
				PicasaExtensions.VirtualFileName (picasaPhoto)
			}, this));
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
			return null;
		}

		protected override VirtualNode NodeInternal (Path path)
		{
			if (path.VirtualPath.Length == 2) {
				return Directory (path);
			} else if (path.VirtualPath.Length == 3) {
				return File (path);
			} else {
				return null;
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

		public static void Register ()
		{
			FileSystemSubsystem fs = new GoogleFileSystem ();

			if (fs != null) {
				FileSystemSubsystems.Subsystems.Add (fs);
				FileSystemSubsystems.DefaultRootDirectory = fs.DefaultRootDirectory;
			}
		}

		public static void AddService (GooglePhotosService service)
		{
			Instance.Services [PicasaExtensions.VirtualDirectoryName (service)] = service;
		}

		public static GoogleFileSystem Instance { get { return FileSystemSubsystems.Subsystems.OfType<GoogleFileSystem> ().First (); } }
	}
}

