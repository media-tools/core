using System;
using Core.Media.Common;
using Core.Shell.Common.FileSystems;

namespace Core.Media.Platform.LocalMedia
{
	public class LocalAlbum : Album
	{
		public LocalAlbum (string name, VirtualDirectory directory)
			: base (name: name)
		{
			Directory = directory;
		}

		#region implemented abstract members of Album

		public override void Load ()
		{
			
		}

		public override Photo InsertPhoto_Internal (Photo photo)
		{
			throw new NotImplementedException ();
		}

		public override Video InsertVideo_Internal (Video video)
		{
			throw new NotImplementedException ();
		}

		#endregion
	}
}

