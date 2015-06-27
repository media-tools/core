using System;
using System.Text.RegularExpressions;
using Core.Common;
using Core.Media.Common;
using Core.Media.Google.GooglePhotos.FileSystem;
using Google.GData.Extensions.MediaRss;
using Google.GData.Photos;
using Picasa = Google.Picasa;

namespace Core.Media.Google.GooglePhotos
{
	public class GoogleContent
	{
		public GoogleFile File { get; private set; }

		public PhotoDimensions Dimensions { get; private set; }

		public string MimeType { get; private set; }

		public string HostedURL { get; private set; }

		public DateTime GoogleTimestamp { get ; private set; }

		public string BestFilename { get ; private set; }

		public string AlternateFilename { get ; private set; }

		public GoogleContent (GooglePhotosService service, DiscretePicasaAlbum picasaAlbum, Picasa.Photo picasaPhoto)
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
				GoogleTimestamp = DateTimeExtensions.FromMillisecondsSinceEpoch (picasaPhoto.Timestamp);
			} catch (OverflowException) {
				GoogleTimestamp = DateTime.Now;
				Log.Debug ("Fuck: ", GoogleTimestamp.ToString ());
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

			if (filename == "MOVIE.m4v") {
				betterFilename = "MOVIE_" + GoogleTimestamp.ToString ("yyyyMMdd_HHmmss") + ".m4v";
			}

			if (!FilenameUtilities.IsPreferredFileName (betterFilename)) {
				DateTime preferredDate;
				// get the date from the filename or use google's timestamp
				DateTime date;
				if (FilenameUtilities.GetFileNameDate (fileName: betterFilename, date: out date) && DateTimeExtensions.HasTimeComponent (date)) {
					preferredDate = date;
				} else {
					preferredDate = GoogleTimestamp;
				}
				betterFilename = FilenameUtilities.MakePreferredFileName (fileName: betterFilename, date: preferredDate, author: username);
			}
			if (FilenameUtilities.HasNoFileEnding (fullPath: betterFilename)) {
				string mimeType = picasaPhoto.PicasaEntry.Content.Type;
				string fileEnding = MimeTypes.ExtensionFromMimeType (mimeType: mimeType);

				// determine the best file ending
				if (fileEnding != null) {
					// rename the file
					filename += fileEnding;
					betterFilename += fileEnding;
					//Log.Debug ("Filename with ending: ", betterFilename);
				}
			}
			betterFilename = regexIllegalCharacters.Replace (betterFilename, "");
			BestFilename = betterFilename;
			AlternateFilename = filename;
			//Log.Debug ("Filename for download: ", BestFilename);
		}

		readonly Regex regexIllegalCharacters = new Regex ("[^a-zA-Z0-9._)( -]");

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

				//Log.Debug ("GoogleContent: ", filename, ": Content: type=", contentType, ", size=", contentWidth, "x", contentHeight);

				if ((contentType.StartsWith ("video") && !MimeType.StartsWith ("video"))
				    || (contentHeight > Dimensions.Height || contentWidth > Dimensions.Width)) {
					Dimensions = new PhotoDimensions { Width = contentWidth, Height = contentHeight };
					MimeType = contentType;
					HostedURL = content.Url;
				}
			}
			//Log.Debug ("GoogleContent: ", filename, ": Best content: type=", MimeType, ", size=", Dimensions.Width, "x", Dimensions.Height);
		}
	}
}
