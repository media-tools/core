using System;
using System.Collections.Generic;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Plus.v1;
using Google.Apis.Plus.v1.Data;
using Google.Apis.Services;
using Core.Common;
using Newtonsoft.Json;

namespace Core.Google.Auth
{
	public class GoogleApp
	{
		public string ClientId { get; private set; }

		public string ClientSecret { get; private set; }

		public static ClientSecretsJson Secrets { get; set; }

		public static readonly string ApplicationName = "Shell.Google";
		// Installed (non-web) application
		public static string RedirectUri = "urn:ietf:wg:oauth:2.0:oob";
		// Requesting access to Contacts API
		public List<GoogleAccount> Accounts = new List<GoogleAccount> ();

		public GoogleApp ()
		{
		}

		public bool Authenticate ()
		{
			return Authenticate (account: null);
		}

	}
}

