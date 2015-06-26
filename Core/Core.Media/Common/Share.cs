using System;
using System.Collections.Generic;

namespace Core.Media.Common
{
	public abstract class Share
	{
		protected Share ()
		{
		}

		public abstract IEnumerable<Album> GetAlbums ();
	}
}

