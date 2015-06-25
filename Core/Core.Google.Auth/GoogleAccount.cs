using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Core.Common;
using Newtonsoft.Json;
using Core.IO;
using Google.Apis.Plus.v1;
using Google.Apis.Services;
using Google.Apis.Plus.v1.Data;

namespace Core.Google.Auth
{
	public class GoogleAccount : ValueObject<GoogleAccount>
	{
		public static string CONFIG_PATH = null;

		GoogleAccountListJson jsonAccounts;
		GoogleAccountJson jsonAccount;

		public string Id { get { return jsonAccount.Id; } }

		public string DisplayName { get { return jsonAccount.DisplayName; } }

		static Regex filterShortDisplayName = new Regex ("[^a-z]");

		public string ShortDisplayName { get { return filterShortDisplayName.Replace (FirstName.ToLower (), ""); } }

		public string FirstName { get { return DisplayName.Contains (" ") ? DisplayName.Substring (0, DisplayName.IndexOf (" ")) : DisplayName; } }

		public string Emails { get { return string.Join (";", jsonAccount.Emails); } }

		public string AccessToken { get { return jsonAccount.AccessToken; } private set { jsonAccount.AccessToken = value; } }

		public string RefreshToken { get { return jsonAccount.RefreshToken; } }

		public GoogleAccount (string id)
			: this ()
		{
			if (!jsonAccounts.Accounts.ContainsKey (id)) {
				jsonAccounts.Accounts [id] = new GoogleAccountJson ();
			}
			jsonAccount = jsonAccounts.Accounts [id];
		}

		private GoogleAccount ()
		{
			jsonAccounts = ConfigHelper.OpenConfig<GoogleAccountListJson> (fullPath: CONFIG_PATH);
		}

		public static GoogleAccount SaveAccount (UserCredential credential, DictionaryDataStore dataStore)
		{
			GoogleAccount dummy = new GoogleAccount ();

			// Create the service.
			PlusService plusService = new PlusService (new BaseClientService.Initializer () {
				HttpClientInitializer = credential,
				ApplicationName = GoogleApp.ApplicationName,
			});

			Person me = plusService.People.Get ("me").Execute ();

			string id = me.Id;
			Log.Info ("Authorized user: ", me.DisplayName);

			GoogleAccountListJson jsonAccounts = dummy.jsonAccounts;
			if (!jsonAccounts.Accounts.ContainsKey (id)) {
				jsonAccounts.Accounts [id] = new GoogleAccountJson ();
			}

			GoogleAccountJson accJson = jsonAccounts.Accounts [id];
			accJson.AccessToken = credential.Token.AccessToken;
			accJson.RefreshToken = credential.Token.RefreshToken;
			accJson.Id = me.Id;
			accJson.Emails = (from email in me.Emails ?? new Person.EmailsData[0]
			                  select email.Value).ToArray ();
			accJson.DisplayName = me.DisplayName;

			ConfigHelper.SaveConfig (fullPath: CONFIG_PATH, stuff: jsonAccounts);

			return new GoogleAccount (id: id);
		}

		public static IEnumerable<GoogleAccount> List ()
		{
			GoogleAccount dummy = new GoogleAccount ();

			string[] ids = dummy.jsonAccounts.Accounts.Keys.ToArray ();
			foreach (string id in ids) {
				GoogleAccount acc = new GoogleAccount (id: id);
				yield return acc;
			}
		}

		public void LoadDataStore (ref DictionaryDataStore dataStore)
		{
			dataStore.Load (dictionary: jsonAccount.DataStore);
		}

		private static string id2section (string id)
		{
			return "Account_" + id;
		}

		public bool Refresh ()
		{
			Core.Net.Networking.DisableCertificateValidation ();

			TokenResponse token = new TokenResponse {
				AccessToken = AccessToken,
				RefreshToken = RefreshToken
			};

			IAuthorizationCodeFlow flow =
				//new GoogleAuthorizationCodeFlow (new GoogleAuthorizationCodeFlow.Initializer {
				new AuthorizationCodeFlow (new AuthorizationCodeFlow.Initializer (GoogleAuthConsts.AuthorizationUrl, GoogleAuthConsts.TokenUrl) {
					ClientSecrets = GoogleApp.Secrets.ToClientSecrets (),
					Scopes = new [] { PlusService.Scope.PlusLogin }
				});

			UserCredential credential = new UserCredential (flow, "me", token);
			bool success;
			try {
				success = credential.RefreshTokenAsync (CancellationToken.None).Result;
			} catch (AggregateException ex) {
				Log.Error ("RefreshTokenAsync failed: ");
				Log.Indent++;
				Log.Error (ex);
				Log.Indent--;
				success = false;
			}

			if (success) {
				token = credential.Token;
				AccessToken = token.AccessToken;
				Log.Debug ("Refresh successful: ", AccessToken);
				return true;
			} else {
				Log.Error ("Refresh failed: ", this);
				return false;
			}
		}

		public bool Reauthenticate ()
		{
			Log.Info ("Google Account needs to be re-authenticated: ", this);
			return new GoogleApp ().Authenticate ();
		}

		public string[] FilterKeys ()
		{
			return new [] { DisplayName, Emails };
		}

		public override string ToString ()
		{
			return string.Format ("{0} <{1}> ({2})", DisplayName, Emails, Id);
		}


		protected override IEnumerable<object> Reflect ()
		{
			return new object[] { Id };
		}

		public sealed class GoogleAccountJson
		{
			[JsonProperty ("access_token")]
			public string AccessToken { get; set; } = "";

			[JsonProperty ("refresh_token")]
			public string RefreshToken { get; set; } = "";

			[JsonProperty ("id")]
			public string Id { get; set; } = "";

			[JsonProperty ("emails")]
			public string[] Emails { get; set; } = new string[0];

			[JsonProperty ("display_name")]
			public string DisplayName { get; set; } = "";

			[JsonProperty ("data_store")]
			public Dictionary<string, string> DataStore { get; set; } = new Dictionary<string, string>();
		}

		public sealed class GoogleAccountListJson
		{
			[JsonProperty ("accounts")]
			public Dictionary<string, GoogleAccountJson> Accounts { get; set; } = new Dictionary<string, GoogleAccountJson>();
		}
	}
}

