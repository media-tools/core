using System;
using Core.Media.Common;
using Google.GData.Photos;
using Picasa = Google.Picasa;

namespace Core.Media.Google.GooglePhotos
{
	public class GoogleAlbum : Album
	{
		readonly GooglePhotosService service;
		readonly PicasaEntry album;

		public GoogleAlbum (GooglePhotosService service, PicasaEntry album, string name)
			: base (name)
		{
			this.service = service;
			this.album = album;
		}

		public override void Load ()
		{
			Picasa.Album picasaAlbum = new Picasa.Album (); 
			picasaAlbum.AtomEntry = album;

			PhotoQuery picturesQuery = new PhotoQuery (PicasaQuery.CreatePicasaUri ("default", picasaAlbum.Id));
			picturesQuery.ExtraParameters = "imgmax=d";
			PicasaFeed picturesFeed = service.PicasaService.Query (picturesQuery);

			foreach (PicasaEntry picture in picturesFeed.Entries) {
				string pictureTitle = picture.Title.Text;

				Picasa.Photo picasaPhoto = new Picasa.Photo ();
				picasaPhoto.AtomEntry = picture;

				GoogleContent content = new GoogleContent (service, picasaAlbum, picasaPhoto);
				if (content.MimeType.StartsWith ("image")) {
					AddPhoto (new GooglePhoto (content));
				} else if (content.MimeType.StartsWith ("video")) {
					AddVideo (new GoogleVideo (content));
				}
			}
		}

		public override void AddPhoto (Photo photo)
		{
			base.AddPhoto (photo);
		}

		public override void AddVideo (Video video)
		{
			base.AddVideo (video);
		}
	}
}

