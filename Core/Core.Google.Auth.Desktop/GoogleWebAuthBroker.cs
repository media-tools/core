using System;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Core.Common;
using System.Threading;
using Google.Apis.Auth.OAuth2.Responses;
using Core.Google.Auth.Portable;

namespace Core.Google.Auth.Desktop
{
	public class GoogleWebAuthBroker : IGoogleAuthBroker
	{
		public GoogleSecrets Secrets { get; }

		public GoogleScopes.ScopeGroup Scopes { get; }

		public List<GoogleAccount> Accounts = new List<GoogleAccount> ();

		public string AppName { get; private set; }

		public GoogleWebAuthBroker (GoogleScopes.ScopeGroup scopes, GoogleSecrets secrets)
		{
			Scopes = scopes;
			Secrets = secrets;

			Core.Net.Networking.DisableCertificateValidation ();
			AppName = Path.GetFileNameWithoutExtension (System.Reflection.Assembly.GetEntryAssembly ().Location);
		}

		public UserCredential Authorize (string googleUser)
		{
			return GoogleWebAuthorizationBroker.AuthorizeAsync (
				GoogleSecrets.ToClientSecrets (Secrets),
				GoogleScopes.ToStrings (Scopes),
				googleUser, System.Threading.CancellationToken.None, 
				new FileDataStore (AppName)
			).Result;
		}

		public bool AuthorizeNew ()
		{
			return Authorize (account: null);
		}

		public bool Authorize (GoogleAccount account)
		{
			DictionaryDataStore dataStore = new DictionaryDataStore ();

			if (account != null) {
				account.LoadDataStore (dataStore: ref dataStore);
			}

			try {
				UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync (
					                            clientSecrets: GoogleSecrets.ToClientSecrets (Secrets),
					                            scopes: GoogleScopes.ToStrings (Scopes),
					                            user: "me",
					                            taskCancellationToken: CancellationToken.None,
					                            dataStore: dataStore
				                            ).Result;
				Accounts.Add (GoogleAccount.SaveAccount (broker: this, credential: credential, dataStore: dataStore));
				return true;
			} catch (TokenResponseException ex) {
				if (ex.Message.Contains ("invalid_grant")) {
					return account.Reauthenticate ();
				} else {
					Log.Error (ex);
				}
				return false;
			}
		}

	}

}

