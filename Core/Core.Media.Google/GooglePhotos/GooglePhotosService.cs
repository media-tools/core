using System;
using Google.GData.Photos;
using Core.Google.Auth.Portable;

namespace Core.Media.Google.GooglePhotos
{
	public sealed class GooglePhotosService : IGoogleAuthReceiver
	{
		public PicasaService PicasaService { get; private set; }

		public GooglePhotosService (GoogleAuthentification auth)
		{
			auth.AddReceiver (this);
		}

		#region IGoogleAuthReceiver implementation


		public void UpdateAuth (GoogleAuthentification auth)
		{
			PicasaService = new PicasaService (auth.RequestFactory.ApplicationName);
			PicasaService.RequestFactory = auth.RequestFactory;
		}

		#endregion
	}
}

