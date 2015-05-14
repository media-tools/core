using System;
using Core.Calendar.Google;
using Core.Common;
using System.Reflection;
using System.Globalization;
using Google.Apis.Auth.OAuth2;
using Core.IO;

namespace Core.Tests.Calendar.Google
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Logging.Enable ();

			GoogleCalendarService google = new GoogleCalendarService (new TestConfig ());
		}

		public class TestConfig : IGoogleConfig
		{
			#region IGoogleConfig implementation

			public string GoogleUser {
				get {
					return "fuck";
				}
			}

			public string CalendarName {
				get {
					throw new NotImplementedException ();
				}
			}

			public IGoogleAuth Auth {
				get {
					throw new NotImplementedException ();
				}
			}

			#endregion


		}
	}
}
