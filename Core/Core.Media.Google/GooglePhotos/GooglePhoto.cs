using System;
using Core.Media.Common;
using Google.GData.Photos;
using Picasa = Google.Picasa;

namespace Core.Media.Google.GooglePhotos
{
	public class GooglePhoto : Photo
	{
		public GooglePhoto (GooglePhotosService service, Picasa.Album picasaAlbum, Picasa.Photo picasaPhoto)
		{
			File = new GoogleFile (service, picasaAlbum, picasaPhoto);

			Dimensions = new PhotoDimensions {
				Width = picasaPhoto.Width,
				Height = picasaPhoto.Height,
			};
		}
	}
}
