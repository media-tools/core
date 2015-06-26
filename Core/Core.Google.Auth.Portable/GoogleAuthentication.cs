using System;
using Core.Common;
using Google.GData.Client;
using System.Collections.Generic;

namespace Core.Google.Auth.Portable
{
	public interface IGoogleAuthReceiver
	{
		// when the tokens change
		void UpdateAuth (GoogleAuthentication auth);
	}

	public sealed class GoogleAuthentication
	{
		// the google app
		public IGoogleAuthBroker Broker { get; set; }
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

		public GoogleAuthentication (IGoogleAuthBroker broker, GoogleAccount account)
		{
			Broker = broker;
			Account = account;
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
				OAuth2Parameters parameters = Account.GetOAuth2Parameters (broker: Broker);

				// for request-based libraries
				Settings = new RequestSettings (Broker.AppName, parameters);

				// for service-based libraries
				RequestFactory = new GOAuth2RequestFactory ("apps", Broker.AppName, parameters);
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

