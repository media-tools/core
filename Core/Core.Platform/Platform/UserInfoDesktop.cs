using System;
using Core.Portable;
using Core.Platform.Windows;

namespace Core.Platform
{
	public static class UserInfoDesktop
	{
		public static void Assign ()
		{
			Console.WriteLine ("UserName: {0}", Environment.UserName);
			Console.WriteLine ("UserDomainName: {0}", Environment.UserDomainName);
			Console.WriteLine ("MachineName: {0}", Environment.MachineName);

			string userShortName = Environment.UserName;
			string hostName = Environment.MachineName;

			string mail;
			if (SystemInfo.IsRunningOnWindows ()) {
				mail = WindowsRegistry.Windows8EmailAddress ();
			} else {
				mail = userShortName + "@" + hostName;
			}

			UserInfo.Assign (
				userShortName: userShortName,
				userFullName: null,
				hostName: hostName,
				userMail: mail
			);
		}
	}
}

