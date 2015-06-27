using System;
using Google.GData.Photos;
using System.Linq;
using System.Collections.Generic;

namespace Core.Media.Google.GooglePhotos.Queries
{
	public class AlbumsQuery
	{
		readonly GooglePhotosService service;
		readonly PicasaEntry[] picasaAlbums;

		public AlbumsQuery (GooglePhotosService service)
		{
			this.service = service;

			AlbumQuery albumsQuery = new AlbumQuery (PicasaQuery.CreatePicasaUri ("default"));
			albumsQuery.ExtraParameters = "imgmax=d";
			PicasaFeed albumsFeed = service.PicasaService.Query (albumsQuery);
			picasaAlbums = albumsFeed.Entries.OfType<PicasaEntry> ().ToArray ();
		}

		public string[] DecodedNames ()
		{
			HashSet<string> albumNames = new HashSet<string> ();
			foreach (PicasaEntry entry in picasaAlbums) {
				DiscretePicasaAlbum album = GoogleNameUtilities.DecodeGoogleAlbumName (picasaAlbum: entry);
				if (album != null) {
					albumNames.Add (album.Name);
				}
			}
			return albumNames.ToArray ();
		}

		public DiscretePicasaAlbum[] ByDecodedName (string decodedName)
		{
			List<DiscretePicasaAlbum> result = new List<DiscretePicasaAlbum> ();
			foreach (PicasaEntry entry in picasaAlbums) {
				DiscretePicasaAlbum album = GoogleNameUtilities.DecodeGoogleAlbumName (picasaAlbum: entry);
				if (album != null && album.Name == decodedName) {
					result.Add (album);
				}
			}
			return result.ToArray ();
		}
	}
}

