using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Core.Media.Common
{
	public static class MimeTypes
	{
		public static bool IsUnknownMimeType (string mimeType)
		{
			return mimeType == "unknown/unknown" || mimeType == "application/binary" || mimeType == "application/octet-stream" || mimeType == "application/unknown";
		}

		static readonly Dictionary<string[], string[]> mimeTypes = new Dictionary<string[], string[]> {
			[ new [] { "image/jpeg" } ] = new [] { ".jpg", ".jpeg", ".pamp" },
			[ new [] { "image/png" } ] = new [] { ".png" },
			[ new [] { "image/gif" } ] = new [] { ".gif" },
			[ new [] { "image/svg+xml" } ] = new [] { ".svg" },
			[ new [] { "image/tiff" } ] = new [] { ".tif", ".tiff" },
			[ new [] { "image/x-ms-bmp" } ] = new [] { ".bmp" },
			[ new [] { "image/x-icon", "image/vnd.microsoft.icon" } ] = new [] { ".ico" },
			[ new [] { "image/x-xcf" } ] = new [] { ".xcf" },

			[ new [] { "video/mpeg4", "video/mp4" } ] = new [] { ".mp4" },
			[ new [] { "video/x-flv" } ] = new [] { ".flv" },
			[ new [] { "video/x-msvideo" } ] = new [] { ".avi" },
			[ new [] { "video/x-matroska" } ] = new [] { ".mkv" },
			[ new [] { "video/webm" } ] = new [] { ".webm" },
			[ new [] { "video/mpeg" } ] = new [] { ".mpg" },
			[ new [] { "video/ogg" } ] = new [] { ".ogv" },
			[ new [] { "video/x-ms-wmv" } ] = new [] { ".wmv" },
			[ new [] { "video/3gpp" } ] = new [] { ".3gp" },

			[ new [] { "audio/mpeg" } ] = new [] { ".mp3" },
			[ new [] { "audio/x-wav" } ] = new [] { ".wav" },
			[ new [] { "audio/ogg" } ] = new [] { ".ogg" },
			[ new [] { "audio/x-ms-wma" } ] = new [] { ".wma" },
			[ new [] { "audio/flac" } ] = new [] { ".flac" },
		};

		static readonly Dictionary<string, string> mimeTypeToExtension = new Dictionary<string, string> ();
		static readonly Dictionary<string, string> extensionToMimeType = new Dictionary<string, string> ();

		static void createMimeTypeDictionary ()
		{
			foreach (var entry in mimeTypes) {
				foreach (string mimeType in entry.Key) {
					mimeTypeToExtension [mimeType] = entry.Value.First ();
				}
				foreach (string extension in entry.Value) {
					extensionToMimeType [extension] = entry.Key.First ();
				}
			}
		}

		public static string MimeTypeFromExtension (string fullPath)
		{
			if (extensionToMimeType.Count == 0) {
				createMimeTypeDictionary ();
			}
			string extension = Path.GetExtension (fullPath);
			return extensionToMimeType.ContainsKey (extension) ? extensionToMimeType [extension.ToLower ()] : null;
		}

		public static string ExtensionFromMimeType (string mimeType)
		{
			if (extensionToMimeType.Count == 0) {
				createMimeTypeDictionary ();
			}
			return mimeTypeToExtension.ContainsKey (mimeType) ? mimeTypeToExtension [mimeType] : null;
		}
	}
}

