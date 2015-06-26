using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Common;

namespace Core.Media.Google.GooglePhotos
{
	public static class FilenameUtilities
	{
		private const string patternGermanDateFormat = "(^|[^0-9])([0-9]{2})[.]([0-9]{2})[.]((?:19|20)[0-9]{2})([^0-9]|$)";

		public static bool IsNormalizedAlbumName (string name)
		{
			if (name == "Winterurlaub 2014")
				return false;
			if (name == "Winterurlaub 2008")
				return false;

			if (name.Contains ("Kamera 20")) {
				return false;
			} else if (name.Length == 0) {
				return false;
			} else if (Regex.IsMatch (name, patternGermanDateFormat)) {
				return false;
			} else {
				return true;
			}
		}

		public static string NormalizeAlbumName (string name)
		{
			if (name == "Winterurlaub 2014")
				return "Winterurlaub 2014-15";
			if (name == "Winterurlaub 2008")
				return "Winterurlaub 2008-09";

			bool done;
			do {
				if (name.Contains ("Kamera 20")) {
					name = Regex.Replace (name, "Kamera", "Auto Backup");
					done = false;
				} else if (name.Length == 0) {
					name = "Unknown";
					done = false;
				} else if (Regex.IsMatch (name, patternGermanDateFormat)) {
					name = Regex.Replace (name, patternGermanDateFormat, "$1$4-$3-$2$5");
					done = false;
				} else {
					done = true;
				}
			} while (!done);
			return name;
		}

		private const string patternDate = "((?:19|20)[0-9]{2})([0-1][0-9])([0-3][0-9])";
		private const string patternTime = "([0-2][0-9])([0-6][0-9])([0-6][0-9])";
		private const string patternShortUserName = "([a-z]+)";

		public static bool IsPreferredFileName (string fileName)
		{
			return Regex.IsMatch (fileName, "^" + patternShortUserName + "_" + patternDate + "_" + patternTime);
		}

		public static string MakePreferredFileName (string fileName, DateTime date, string author)
		{
			if (IsPreferredFileName (fileName)) {
				return fileName;
			} else {
				// add the date to the filename, if it's not already there!
				string dateStr = date.ToString ("yyyyMMdd_HHmmss");
				if (!fileName.StartsWith (dateStr + "_")) {
					if (fileName.StartsWith (dateStr)) {
						fileName = fileName.Substring (dateStr.Length);
						if (fileName.StartsWith (".")) {
							fileName = "image" + fileName;
						}
					}
					fileName = dateStr + "_" + fileName;
				}
				// add the author's name to the filename, if it's not already there!
				if (!fileName.StartsWith (author)) {
					fileName = author + "_" + fileName;
				}
				return fileName;
			}
		}

		public static bool IsRawFilename (string fileName)
		{
			return Path.GetFileName (fileName).StartsWith ("raw_");
		}

		public static string MakeRawFilename (string fileName)
		{
			return IsRawFilename (fileName) ? fileName : Path.Combine (Path.GetDirectoryName (fileName), "raw_" + Path.GetFileName (fileName));
		}

		public static string UnmakeRawFilename (string fileName)
		{
			return IsRawFilename (fileName) ? Path.Combine (Path.GetDirectoryName (fileName), Path.GetFileName (fileName).Substring (4)) : fileName;
		}


		class FormatCombo
		{
			public string Prefix;
			public string Format;
		}

		static FormatCombo[] dateFormats = new FormatCombo[] {
			new FormatCombo { Prefix = "", Format = "yyyy_MMdd_HHmmss" },
			new FormatCombo { Prefix = "", Format = "yyyyMMdd_HHmmss" },
			new FormatCombo { Prefix = "", Format = "yyyyMMdd-HHmmss" },
			new FormatCombo { Prefix = "", Format = "yyyyMMdd HHmmss" },
			new FormatCombo { Prefix = "", Format = "yyyyMMdd" },
			new FormatCombo { Prefix = "IMG_", Format = "yyyyMMdd_HHmmss" },
			new FormatCombo { Prefix = "IMG_", Format = "ddMMyyyy_HHmmss" },
			new FormatCombo { Prefix = "IMG-", Format = "yyyyMMdd-HHmmss" },
			new FormatCombo { Prefix = "IMG-", Format = "yyyyMMdd" },
			new FormatCombo { Prefix = "Screenshot_", Format = "yyyy-MM-dd-HH-mm-ss" },
			new FormatCombo { Prefix = "Screenshot_", Format = "yyyy-MM-dd HH.mm.ss" },
			new FormatCombo { Prefix = "Screenshot ", Format = "yyyy-MM-dd HH.mm.ss" },
			new FormatCombo { Prefix = "Screenshot - ", Format = "dd.MM.yyyy - HH_mm_ss" },
			new FormatCombo { Prefix = "Bildschirmfoto vom ", Format = "yyyy-MM-dd HH_mm_ss" },
			new FormatCombo { Prefix = "Bildschirmfoto - ", Format = "dd.MM.yyyy - HH_mm_ss" },
			new FormatCombo { Prefix = "", Format = "yyyy-MM-dd HH-mm-ss" },
			new FormatCombo { Prefix = "", Format = "yyyy-MM-dd HH.mm.ss" },
			new FormatCombo { Prefix = "", Format = "yyyy-MM-dd HHmmss" },

			new FormatCombo { Prefix = "", Format = "yyyy-MM-dd-HH-mm-ss" },
			new FormatCombo { Prefix = "", Format = "yyyy-MM-dd-HH_mm_ss" },
			new FormatCombo { Prefix = "", Format = "yyyy-MM-dd-HH.mm.ss" },
			new FormatCombo { Prefix = "", Format = "yyyy-MM-dd-HHmmss" },
			new FormatCombo { Prefix = "", Format = "yyyy-MM-dd_HH-mm-ss" },
			new FormatCombo { Prefix = "", Format = "yyyy-MM-dd_HH_mm_ss" },
			new FormatCombo { Prefix = "", Format = "yyyy-MM-dd_HH.mm.ss" },
			new FormatCombo { Prefix = "", Format = "yyyy-MM-dd_HHmmss" },
			new FormatCombo { Prefix = "", Format = "yyyy_MM_dd_HH-mm-ss" },
			new FormatCombo { Prefix = "", Format = "yyyy_MM_dd_HH_mm_ss" },
			new FormatCombo { Prefix = "", Format = "yyyy_MM_dd_HH.mm.ss" },
			new FormatCombo { Prefix = "", Format = "yyyy_MM_dd_HHmmss" },
			new FormatCombo { Prefix = "", Format = "yyyy.MM.dd.HH-mm-ss" },
			new FormatCombo { Prefix = "", Format = "yyyy.MM.dd.HH_mm_ss" },
			new FormatCombo { Prefix = "", Format = "yyyy.MM.dd.HH.mm.ss" },
			new FormatCombo { Prefix = "", Format = "yyyy.MM.dd.HHmmss" },
			new FormatCombo { Prefix = "", Format = "yyyy-MM-dd" },
		};

		public static bool GetFileNameDate (string fileName, out DateTime date)
		{
			foreach (FormatCombo formatCombo in dateFormats) {
				string prefix = formatCombo.Prefix;
				string format = formatCombo.Format;
				string regexPrefix = prefix;
				string regexFormat = format.Replace ("yyyy", "(?:19|20)[0-9][0-9]").Replace ("MM", "[0-1][0-9]").Replace ("dd", "[0-3][0-9]").Replace ("HH", "[0-9][0-9]")
						.Replace ("mm", "[0-9][0-9]").Replace ("ss", "[0-9][0-9]").Replace (".", "\\.");

				foreach (string preprefix in new [] { "^", "" }) {
					Regex regex = new Regex (preprefix + regexPrefix + "(" + regexFormat + ")");
					// Log.Debug ("prefix=", prefix, ", format=", format, ", regex=", regex);
					Match match = regex.Match (fileName);
					if (match.Success) {
						string dateTimeStr = match.Groups [1].Value;
						Log.Debug ("GetFileNameDate: fileName=", fileName, ", dateTimeStr=", dateTimeStr, " (TryParseExact)");
						if (DateTime.TryParseExact (dateTimeStr, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out date)) {
							if (date != DateTime.MinValue) {
								return true;
							}
						}
					}
				}

				foreach (string preprefix in new [] { "^", "" }) {
					Regex regex = new Regex (preprefix + regexPrefix + "(" + regexFormat + ")");
					// Log.Debug ("prefix=", prefix, ", format=", format, ", regex=", regex);
					Match match = regex.Match (fileName);
					if (match.Success) {
						string dateTimeStr = match.Groups [1].Value;
						Log.Debug ("GetFileNameDate: fileName=", fileName, ", dateTimeStr=", dateTimeStr, " (TryParse)");

						if (DateTime.TryParse (dateTimeStr, out date)) {
							if (date != DateTime.MinValue) {
								return true;
							}
						}
					}
				}
			}
			date = DateTime.MinValue;
			return false;
		}

		public static bool HasNoFileEnding (string fullPath)
		{
			bool hasNoEnding;

			string fileName = fullPath.Split ('/').Last ();
			string lastPart = fileName.Split ('.').Last ();
			bool containsLetters = lastPart.Any (x => char.IsLetter (x));
			bool containsWhitespaces = lastPart.Any (x => char.IsWhiteSpace (x));
			bool containsPunctuation = lastPart.Any (x => char.IsPunctuation (x));

			hasNoEnding = !containsLetters || containsWhitespaces || containsPunctuation;

			if (hasNoEnding) {
				Log.Debug ("File has no extension: ", fileName);
				Log.Indent++;
				Log.Debug ("HasNoFileEnding: result=", hasNoEnding, ", fileName=", fileName, ", lastPart=", lastPart, ", containsLetters=", containsLetters, ", containsWhitespaces=", containsWhitespaces, ", containsPunctuation=", containsPunctuation);
				Log.Indent--;
			}

			return hasNoEnding;
		}

	}
}