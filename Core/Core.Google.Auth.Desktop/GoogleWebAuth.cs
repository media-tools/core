using System;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using Core.Calendar.Google;

namespace Core.Google.Auth.Desktop
{
	public class GoogleWebAuth : IGoogleAuth
	{
		internal readonly string ClientId;
		internal readonly string ClientSecret;

		public GoogleWebAuth (string clientId, string clientSecret)
		{
			ClientId = clientId;
			ClientSecret = clientSecret;
		}

		public UserCredential Authorize (string googleUser)
		{
			return GoogleWebAuthorizationBroker.AuthorizeAsync (
				new ClientSecrets {
					ClientId = ClientId,
					ClientSecret = ClientSecret
				},
				new[] { "https://www.googleapis.com/auth/calendar" },
				googleUser, System.Threading.CancellationToken.None, 
				new FileDataStore ("Drive.Auth.Store")
			).Result;
		}
	}
}

