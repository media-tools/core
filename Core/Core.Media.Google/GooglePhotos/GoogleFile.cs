using System;
using Core.Media.Common;
using Core.Shell.Common.FileSystems;
using Google.GData.Photos;
using Picasa = Google.Picasa;

namespace Core.Media.Google.GooglePhotos
{
	public class GoogleFile : VirtualFile
	{
		public GoogleFile (Path path)
			: base (path)
		{
			
		}
	}

}
