using System;
using Core.Common;
using Google.GData.Client;
using System.Collections.Generic;

namespace Core.Google.Auth
{
	public interface IGoogleAuthReceiver
	{
		// when the tokens change
		Action UpdateAuth (GoogleAuth auth);
	}

	public sealed class GoogleAuth
	{
		// the google app
		public GoogleApp App { get; set; }
		// the google account
		public GoogleAccount Account { get; set; }

		// for request-based libraries
		public RequestSettings Settings { get; set; }

		// for service-based libraries
		public GOAuth2RequestFactory RequestFactory { get; set; }

		// internal lock
		object internalLockAuth = new object ();

		// clients
		List<IGoogleAuthReceiver> Receivers = new List<IGoogleAuthReceiver> ();

		public GoogleAuth (GoogleApp app, GoogleAccount account)
		{
			App = app;
			Account = account;

			Core.Net.Networking.DisableCertificateValidation ();
		}

		public void AddReceiver (params IGoogleAuthReceiver[] receivers)
		{
			Receivers.AddRange (receivers);

			UpdateAuthInternal ();
		}

		private void UpdateAuthInternal ()
		{
			lock (internalLockAuth) {
				// get the OAuth2 parameters
				OAuth2Parameters parameters = Account.GetOAuth2Parameters (app: App);

				// for request-based libraries
				Settings = new RequestSettings (GoogleApp.ApplicationName, parameters);

				// for service-based libraries
				RequestFactory = new GOAuth2RequestFactory ("apps", GoogleApp.ApplicationName, parameters);
				RequestFactory.MethodOverride = true;

				// call the method implemented in the derived class
				foreach (var r in Receivers) {
					r.UpdateAuth (this);
				}
			}
		}

		public void RefreshAccount ()
		{
			Log.Debug ("Refresh account authorization.");
			Log.Indent++;
			Account.Refresh ();
			UpdateAuthInternal ();
			Log.Indent--;
		}
	}
}

