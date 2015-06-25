using System;
using Core.Common;
using Core.Google.Auth.Portable;
using Google.Apis.Auth.OAuth2;

namespace Core.Calendar.Google
{
	public interface IGoogleConfig
	{
		string GoogleUser { get; }

		IGoogleAuthBroker Auth { get; }

		string CalendarName { get; }
	}
}

