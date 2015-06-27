using System;
using Google.GData.Photos;
using Picasa = Google.Picasa;
using System.Linq;
using System.Collections.Generic;

namespace Core.Media.Google.GooglePhotos.Queries
{
	public class ContentQuery
	{
		readonly GooglePhotosService service;
		readonly PicasaEntry[] picasaPictures;
		readonly DiscretePicasaAlbum album;

		public ContentQuery (GooglePhotosService service, DiscretePicasaAlbum album)
		{
			this.service = service;
			this.album = album;

			Picasa.Album picasaAlbum = new Picasa.Album (); 
			picasaAlbum.AtomEntry = album.Entry;

			PhotoQuery picturesQuery = new PhotoQuery (PicasaQuery.CreatePicasaUri ("default", picasaAlbum.Id));
			picturesQuery.ExtraParameters = "imgmax=d";
			PicasaFeed picturesFeed = service.PicasaService.Query (picturesQuery);
			picasaPictures = picturesFeed.Entries.OfType<PicasaEntry> ().ToArray ();
		}

		public GoogleContent[] Content ()
		{
			List<GoogleContent> result = new List<GoogleContent> ();
			foreach (PicasaEntry picture in picasaPictures) {
				string pictureTitle = picture.Title.Text;

				Picasa.Photo picasaPhoto = new Picasa.Photo ();
				picasaPhoto.AtomEntry = picture;

				GoogleContent content = new GoogleContent (service, album, picasaPhoto);
				result.Add (content);
			}
			return result.ToArray ();
		}
	}
}
