using System;
using System.Collections.Generic;
using System.Linq;
using Core.Google.Auth.Portable;
using Core.Media.Common;
using Core.Media.Google.GooglePhotos.Queries;
using Google.GData.Photos;

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
			AlbumsQuery albumsQuery = new AlbumsQuery (service);
			string[] albumNames = albumsQuery.DecodedNames ();

			List<GoogleAlbum> result = new List<GoogleAlbum> ();
			foreach (string albumName in albumNames) {
				result.Add (new GoogleAlbum (service: service, albumsQuery: albumsQuery, name: albumName));
			}

			return result.OrderBy (a => a.Name);
		}
	}
}