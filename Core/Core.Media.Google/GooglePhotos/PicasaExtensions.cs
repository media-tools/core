using System;
using System.Collections.Generic;
using System.Linq;
using Core.IO;
using Core.Shell.Common.FileSystems;
using Picasa = Google.Picasa;

namespace Core.Media.Google.GooglePhotos
{
	public static class PicasaExtensions
	{
		public static string VirtualDirectoryName (GooglePhotosService service)
		{
			return service.Auth.Account.Emails;
		}

		public static string VirtualDirectoryName (Picasa.Album picasaAlbum)
		{
			return picasaAlbum.Title + " (" + picasaAlbum.Id + ")";
		}

		public static string VirtualFileName (Picasa.Photo picasaPhoto)
		{
			return picasaPhoto.Title + " (" + picasaPhoto.Id + ")";
		}
	}
}
