using System;
using Core.Calendar.Google;
using Core.Platform;

namespace Core.Tests.Calendar.Google
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			DesktopPlatform.Start ();

			GoogleCalendarService google = new GoogleCalendarService (new TestConfig ());

			DesktopPlatform.Finish ();
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
