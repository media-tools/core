using System;
using System.Collections.Generic;
using Core.Google.Auth.Portable;
using Core.Media.Common;
using Google.GData.Photos;

namespace Core.Media.Google.GooglePhotos
{
	public class GoogleShare : Share, IGoogleAuthReceiver
	{
		private PicasaService service;

		public GoogleShare (GoogleAuthentification auth)
		{
			auth.AddReceiver (this);
		}

		#region IGoogleAuthReceiver implementation


		public void UpdateAuth (GoogleAuthentification auth)
		{
			service = new PicasaService (auth.RequestFactory.ApplicationName);
			service.RequestFactory = auth.RequestFactory;
		}


		#endregion

		public override IEnumerable<Album> GetAlbums ()
		{
			AlbumQuery albumsQuery = new AlbumQuery (PicasaQuery.CreatePicasaUri ("default"));
			albumsQuery.ExtraParameters = "imgmax=d";
			PicasaFeed albumsFeed = service.Query (albumsQuery);

			//Loop through all albums
			int albumCount = 0;
			int pictureCount = 0;
			long totalSize = 0;
			foreach (PicasaEntry album in albumsFeed.Entries) {
				string albumTitle = album.Title.Text;
				string albumPath = System.IO.Path.Combine ("", albumTitle);

				yield return  new GoogleAlbum (albumTitle);
			}
		}
	}
}