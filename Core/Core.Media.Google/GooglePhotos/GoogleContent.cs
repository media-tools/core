using System;
using Core.Media.Common;
using Google.GData.Photos;
using Picasa = Google.Picasa;
using Google.GData.Extensions.MediaRss;
using Core.Common;

namespace Core.Media.Google.GooglePhotos
{
	public class GoogleContent
	{
		public GoogleFile File { get; private set; }

		public PhotoDimensions Dimensions { get; private set; }

		public string MimeType { get; private set; }

		public string HostedURL { get; private set; }

		public ulong GoogleTimestampUnix { get; private set; }

		public DateTime GoogleTimestamp { get { return DateTimeExtensions.MillisecondsTimeStampToDateTime (TimestampUnix); } }


		public GoogleContent (GooglePhotosService service, Picasa.Album picasaAlbum, Picasa.Photo picasaPhoto)
		{
			File = GoogleFileSystem.Instance.File (service, picasaAlbum, picasaPhoto);

			Dimensions = new PhotoDimensions {
				Width = picasaPhoto.Width,
				Height = picasaPhoto.Height,
			};

			findBestContent (picasaPhoto);
			findTimestamp (picasaPhoto);
			findFilename (service, picasaPhoto);
		}

		void findTimestamp (Picasa.Photo picasaPhoto)
		{
			try {
				TimestampUnix = internalPhoto.Timestamp;
			} catch (OverflowException) {
				TimestampUnix = (ulong)DateTime.Now.ToMillisecondsTimestamp ();
				Log.Debug ("Fuck: ", Timestamp.ToString ());
			}
		}

		void findFilename (GooglePhotosService service, Picasa.Photo picasaPhoto)
		{
			string filename = picasaPhoto.Title;
			// the file name has to be platform independent
			filename = filename.Replace (":", "_").Trim ('_', '.', ' ', '~');
			// the file ending has to be in lower case
			filename = System.IO.Path.GetFileNameWithoutExtension (filename) + System.IO.Path.GetExtension (filename).ToLower ();

			string betterFilename = filename;
			string username = service.Auth.Account.ShortDisplayName;

			if (Filename == "MOVIE.m4v") {
				betterFilename = "MOVIE_" + Timestamp.ToString ("yyyyMMdd_HHmmss") + ".m4v";
			}
		}

		void findBestContent (Picasa.Photo picasaPhoto)
		{
			string filename = picasaPhoto.Title;
			MimeType = picasaPhoto.PicasaEntry.Content.Type;
			HostedURL = picasaPhoto.PicasaEntry.Content.AbsoluteUri;

			foreach (MediaContent content in (picasaPhoto.AtomEntry as PicasaEntry).Media.Contents) {
				string contentType = content.Type;
				int contentWidth = 0;
				int contentHeight = 0;

				if (!int.TryParse (content.Width, out contentWidth)) {
					Log.Debug ("GoogleContent: ", filename, ": Failed to parse int (width): ", content.Width);
				}
				if (!int.TryParse (content.Height, out contentHeight)) {
					Log.Debug ("GoogleContent: ", filename, ": Failed to parse int (height): ", content.Height);
				}

				Log.Debug ("GoogleContent: ", filename, ": Content: type=", contentType, ", size=", contentWidth, "x", contentHeight);

				if ((contentType.StartsWith ("video") && !MimeType.StartsWith ("video"))
				    || (contentHeight > Dimensions.Height || contentWidth > Dimensions.Width)) {
					Dimensions = new PhotoDimensions { Width = contentWidth, Height = contentHeight };
					MimeType = contentType;
					HostedURL = content.Url;
				}
			}
			Log.Debug ("GoogleContent: ", filename, ": Best content: type=", MimeType, ", size=", Dimensions.Width, "x", Dimensions.Height);
		}
	}
}
