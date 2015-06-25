using System;
using Core.Media.Common;
using System.Collections.Generic;
using Google.GData.Photos;

namespace Core.Media.Google.GooglePhotos
{
	public class GoogleShare : Share, IGoogleAuthReceiver
	{
		public GoogleShare ()
		{
		}

		public override IEnumerable<Album> GetAlbums ()
		{
			AlbumQuery albumsQuery = new AlbumQuery (PicasaQuery.CreatePicasaUri ("default"));
			albumsQuery.ExtraParameters = "imgmax=d";
			PicasaFeed albumsFeed = picasaService.Query (albumsQuery);

			//Loop through all albums
			int albumCount = 0;
			int pictureCount = 0;
			long totalSize = 0;
			foreach (PicasaEntry album in albumsFeed.Entries) {
				string albumTitle = album.Title.Text;
				string albumPath = Path.Combine (Arguments.Destination, albumTitle);

			}
		}
	}
}