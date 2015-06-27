using System;
using Core.Media.Common;
using System.Text.RegularExpressions;
using Google.GData.Photos;

namespace Core.Media.Google.GooglePhotos
{
	public static class GoogleNameUtilities
	{
		public static string SPECIAL_ALBUM_AUTO_BACKUP = "Auto Backup";
		public static string SPECIAL_ALBUM_HANGOUT = "Hangout:";
		public static string SPECIAL_ALBUM_DATE_TITLE_REGEX = "^(((?:19|20)[0-9]{2})-([0-1][0-9])-([0-3][0-9]))|(([0-3][0-9])[.]([0-1][0-9])[.]([0-9]{2}))$";

		public static DiscretePicasaAlbum DecodeGoogleAlbumName (PicasaEntry picasaAlbum)
		{
			string input = picasaAlbum.Title.Text;
			Regex regex;
			Match match;

			regex = new Regex (@"\[([^\]]+)\] \(([0-9]+)\)");
			match = regex.Match (input);
			if (match.Success) {
				string albumName = match.Groups [1].Value;
				int albumIndex = int.Parse (match.Groups [2].Value);
				return new DiscretePicasaAlbum (entry: picasaAlbum, name: albumName, index: albumIndex);
			}

			regex = new Regex (@"\[([^\]]+)\]");
			match = regex.Match (input);
			if (match.Success) {
				string albumName = match.Groups [1].Value;
				const int albumIndex = 1;
				return new DiscretePicasaAlbum (entry: picasaAlbum, name: albumName, index: albumIndex);
			}

			if (input == SPECIAL_ALBUM_AUTO_BACKUP || input.StartsWith (SPECIAL_ALBUM_HANGOUT)) {
				string albumName = input;
				const int albumIndex = 1;
				return new DiscretePicasaAlbum (entry: picasaAlbum, name: albumName, index: albumIndex);
			}

			return null;
		}

		public static string EncodeGoogleAlbumName (string albumName, int index)
		{
			string input = albumName;

			if (input == SPECIAL_ALBUM_AUTO_BACKUP || input.StartsWith (SPECIAL_ALBUM_HANGOUT)) {
				return input;
			}

			if (index == 1) {
				return "[" + input + "]"; 
			} else {
				return "[" + input + "] (" + index + ")"; 
			}
		}

		/*
		public static string SYNCED_ALBUM_PREFIX = "[";
		public static string SYNCED_ALBUM_SUFFIX = "]";

		public static bool IsSyncedAlbum (GoogleAlbum album)
		{
			return album.Name.StartsWith (SYNCED_ALBUM_PREFIX) && album.Name.EndsWith (SYNCED_ALBUM_SUFFIX);
		}

		public static string ToSyncedAlbumName (string albumName)
		{
			return SYNCED_ALBUM_PREFIX + albumName + SYNCED_ALBUM_SUFFIX;
		}

		public static bool IsIncludedInSync (Album album)
		{
			string name = album.Name;
			return name.Length > 0 && !Regex.IsMatch (name, "(^|/)[0-9]{4}[/-][0-9]{2}[/-][0-9]{2}");
		}
		*/
	}
}

