using System;
using Core.Calendar.Google;

namespace Core.Tests.Calendar.Google
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			GoogleBindingRedirect.Apply ();

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

			#endregion

		}
	}
}
