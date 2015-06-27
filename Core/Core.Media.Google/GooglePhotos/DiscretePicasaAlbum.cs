using System;
using Core.Media.Common;
using Core.Media.Google.GooglePhotos.Queries;
using Google.GData.Photos;
using Picasa = Google.Picasa;

namespace Core.Media.Google.GooglePhotos
{
	public class DiscretePicasaAlbum
	{
		public readonly PicasaEntry Entry;
		public readonly string Name;
		public readonly int Index;

		public DiscretePicasaAlbum (PicasaEntry entry, string name, int index)
		{
			Entry = entry;
			Name = name;
			Index = index;
		}
	}
}
