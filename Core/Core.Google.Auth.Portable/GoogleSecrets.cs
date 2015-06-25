using System;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;

namespace Core.Google.Auth.Portable
{
	public sealed class GoogleSecrets
	{
		[JsonProperty ("client_id")]
		public string ClientId { get; set; } = null;

		[JsonProperty ("client_secret")]
		public string ClientSecret { get; set; } = null;

		public static ClientSecrets ToClientSecrets (GoogleSecrets secrets)
		{
			return new ClientSecrets {
				ClientId = secrets.ClientId,
				ClientSecret = secrets.ClientSecret,
			};
		}
	}
}
