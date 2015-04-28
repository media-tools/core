using System;
using Core.Common;
using Google.Apis.Auth.OAuth2;

namespace Core.Calendar.Google
{
	public interface IGoogleConfig
	{
		string GoogleUser { get; }

		IGoogleAuth Auth { get; }

		string CalendarName { get; }
	}
}

