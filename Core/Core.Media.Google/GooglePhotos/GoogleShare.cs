using System;
using System.Collections.Generic;
using Core.Google.Auth.Portable;
using Core.Media.Common;
using Google.GData.Photos;
using System.Linq;

namespace Core.Media.Google.GooglePhotos
{
	public class GoogleShare : Share
	{
		readonly GooglePhotosService service;

		public GoogleShare (GooglePhotosService service)
		{
			this.service = service;
		}

		public override IEnumerable<Album> GetAlbums ()
		{
			AlbumQuery albumsQuery = new AlbumQuery (PicasaQuery.CreatePicasaUri ("default"));
			albumsQuery.ExtraParameters = "imgmax=d";
			PicasaFeed albumsFeed = service.PicasaService.Query (albumsQuery);

			List<GoogleAlbum> result = new List<GoogleAlbum> ();
			//Loop through all albums
			foreach (PicasaEntry album in albumsFeed.Entries) {
				string albumTitle = album.Title.Text;

				result.Add (new GoogleAlbum (service: service, album: album, name: albumTitle));
			}

			return result.OrderBy (a => a.Name);
		}
	}
}