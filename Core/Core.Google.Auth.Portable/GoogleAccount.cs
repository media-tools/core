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
using Google.GData.Client;

namespace Core.Google.Auth.Portable
{
	public sealed class GoogleAccount : ValueObject<GoogleAccount>
	{
		public static IDisposableConfigHelper<GoogleAccountListJson> Config = null;

		public static string RedirectUri = "urn:ietf:wg:oauth:2.0:oob";

		IGoogleAuthBroker Broker;

		GoogleAccountJson jsonAccount;

		public string Id { get { return jsonAccount.Id; } }

		public string DisplayName { get { return jsonAccount.DisplayName; } }

		static Regex filterShortDisplayName = new Regex ("[^a-z]");

		public string ShortDisplayName { get { return filterShortDisplayName.Replace (FirstName.ToLower (), ""); } }

		public string FirstName { get { return DisplayName.Contains (" ") ? DisplayName.Substring (0, DisplayName.IndexOf (" ")) : DisplayName; } }

		public string Emails { get { return string.Join (";", jsonAccount.Emails); } }

		public string AccessToken { get { return jsonAccount.AccessToken; } private set { jsonAccount.AccessToken = value; } }

		public string RefreshToken { get { return jsonAccount.RefreshToken; } }

		public GoogleAccount (IGoogleAuthBroker broker, string id)
			: this ()
		{
			Broker = broker;

			using (var json = Config) {
				if (!json.Value.Accounts.ContainsKey (id)) {
					json.Value.Accounts [id] = new GoogleAccountJson ();
				}
				jsonAccount = json.Value.Accounts [id];
			}
		}

		private GoogleAccount ()
		{
		}

		public static GoogleAccount SaveAccount (IGoogleAuthBroker broker, UserCredential credential, DictionaryDataStore dataStore)
		{
			// Create the service.
			PlusService plusService = new PlusService (new BaseClientService.Initializer () {
				HttpClientInitializer = credential,
				ApplicationName = broker.AppName,
			});

			Person me = plusService.People.Get ("me").Execute ();

			string id = me.Id;
			Log.Info ("Authorized user: ", me.DisplayName);

			using (var json = Config) {
				if (!json.Value.Accounts.ContainsKey (id)) {
					json.Value.Accounts [id] = new GoogleAccountJson ();
				}

				GoogleAccountJson accJson = json.Value.Accounts [id];
				accJson.AccessToken = credential.Token.AccessToken;
				accJson.RefreshToken = credential.Token.RefreshToken;
				accJson.Id = me.Id;
				accJson.Emails = (from email in me.Emails ?? new Person.EmailsData[0]
				                  select email.Value).ToArray ();
				accJson.DisplayName = me.DisplayName;
			}

			return new GoogleAccount (broker: broker, id: id);
		}

		public static IEnumerable<GoogleAccount> List (IGoogleAuthBroker broker)
		{
			using (var json = Config) {
				string[] ids = json.Value.Accounts.Keys.ToArray ();
				foreach (string id in ids) {
					GoogleAccount acc = new GoogleAccount (broker: broker, id: id);
					yield return acc;
				}
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
			TokenResponse token = new TokenResponse {
				AccessToken = AccessToken,
				RefreshToken = RefreshToken
			};

			IAuthorizationCodeFlow flow =
				new AuthorizationCodeFlow (new AuthorizationCodeFlow.Initializer (GoogleAuthConsts.AuthorizationUrl, GoogleAuthConsts.TokenUrl) {
					ClientSecrets = GoogleSecrets.ToClientSecrets (Broker.Secrets),
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
			return Broker.AuthorizeNew ();
		}

		public OAuth2Parameters GetOAuth2Parameters (IGoogleAuthBroker broker)
		{
			OAuth2Parameters parameters = new OAuth2Parameters {
				ClientId = broker.Secrets.ClientId,
				ClientSecret = broker.Secrets.ClientSecret,
				RedirectUri = RedirectUri,
				Scope = string.Join (" ", broker.Scopes),
			};
			parameters.AccessToken = AccessToken;
			parameters.RefreshToken = RefreshToken;

			return parameters;
		}

		public override string ToString ()
		{
			return string.Format ("{0} <{1}> ({2})", DisplayName, Emails, Id);
		}


		protected override IEnumerable<object> Reflect ()
		{
			return new object[] { Id };
		}

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

