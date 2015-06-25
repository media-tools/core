using System;
using Google.Apis.Auth.OAuth2;

namespace Core.Google.Auth.Portable
{
	public interface IGoogleAuthBroker
	{
		string AppName { get; }

		GoogleSecrets Secrets { get; }

		GoogleScopes.ScopeGroup Scopes { get; }

		bool AuthorizeNew ();

		bool Authorize (GoogleAccount account);

		UserCredential Authorize (string googleUser);
	}
}

