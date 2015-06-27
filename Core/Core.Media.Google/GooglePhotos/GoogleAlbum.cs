using System;
using Core.Media.Common;
using Core.Media.Google.GooglePhotos.Queries;
using Google.GData.Photos;
using Picasa = Google.Picasa;

namespace Core.Media.Google.GooglePhotos
{
	public sealed class GoogleAlbum : Album
	{
		readonly GooglePhotosService service;
		readonly AlbumsQuery albumsQuery;

		DiscretePicasaAlbum[] picasaAlbums;

		public GoogleAlbum (GooglePhotosService service, AlbumsQuery albumsQuery, string name)
			: base (name)
		{
			this.service = service;
			this.albumsQuery = albumsQuery;

			Directory = null;

			picasaAlbums = albumsQuery.ByDecodedName (decodedName: Name);
		}

		#region implemented abstract members of Album

		public override void Load ()
		{
			foreach (DiscretePicasaAlbum album in picasaAlbums) {
				ContentQuery contentQuery = new ContentQuery (service, album);

				foreach (GoogleContent content in contentQuery.Content()) {
					if (content.MimeType.StartsWith ("image")) {
						AddPhoto (new GooglePhoto (content));
					} else if (content.MimeType.StartsWith ("video")) {
						AddVideo (new GoogleVideo (content));
					}
				}
			}
		}

		public override Photo InsertPhoto_Internal (Photo photo)
		{
			throw new NotImplementedException ();
		}

		public override Video InsertVideo_Internal (Video video)
		{
			throw new NotImplementedException ();
		}

		#endregion
	}

}

