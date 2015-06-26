using Core.Media.Common;
using Picasa = Google.Picasa;

namespace Core.Media.Google.GooglePhotos
{
	public class GoogleVideo : Video
	{
		public GoogleVideo (GoogleContent content)
		{
			File = content.File;
			Dimensions = content.Dimensions;
			HostedURL = content.HostedURL;
			MimeType = content.MimeType;
		}
	}
}
