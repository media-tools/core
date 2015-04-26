using System;
using Google.Apis.Auth.OAuth2;

namespace Core.Calendar.Google
{
	public interface IGoogleAuth
	{
		UserCredential Authorize (string googleUser);
	}
}

