using System;
using System.Collections.Generic;

namespace Core.Google.Auth.Portable
{
	public static class GoogleScopes
	{
		public static string[] ToStrings (ScopeGroup scopeGroup)
		{
			return scopesDict [scopeGroup];
		}

		readonly static Dictionary<ScopeGroup, string[]> scopesDict = new Dictionary<ScopeGroup, string[]> {
			[ScopeGroup.Calendar ] = 
				new[] { "https://www.googleapis.com/auth/calendar" },
			[ScopeGroup.All ] = 
				new[] {
				"https://www.googleapis.com/auth/plus.login",
				"https://www.googleapis.com/auth/plus.me",
				"profile",
				"email",

				"https://www.google.com/m8/feeds",
				"https://picasaweb.google.com/data/",

				"https://www.googleapis.com/auth/youtube",
				"https://www.googleapis.com/auth/youtube.upload",
				"https://www.googleapis.com/auth/plus.circles.read",
				"https://www.googleapis.com/auth/plus.circles.write",
				"https://www.googleapis.com/auth/plus.stream.read",
				"https://www.googleapis.com/auth/plus.stream.write",
				"https://www.googleapis.com/auth/plus.media.upload",
				"https://mail.google.com/",
				"https://www.googleapis.com/auth/drive",
			}
		};

		public enum ScopeGroup
		{
			All,
			Calendar
		}
	}
}

