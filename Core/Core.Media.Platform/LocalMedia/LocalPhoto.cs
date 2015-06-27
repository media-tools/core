using System;
using Core.Media.Common;
using Core.Shell.Platform.FileSystems;

namespace Core.Media.Platform.LocalMedia
{
	public class LocalPhoto : Photo
	{
		public LocalPhoto (LocalAlbum album, string filename)
		{
			File = (album.Directory as RegularDirectory).GetChildFile (filename);
			//Dimensions = content.Dimensions;
			//HostedURL = content.HostedURL;
			//MimeType = content.MimeType;
		}
	}
}

