using System;
using Microsoft.Win32;
using Core.Common;

namespace Core.Platform.Windows
{
	public static class WindowsRegistry
	{
		internal static void SetAssociation (string[] extensions, string id, string description, string exePath, string iconPath)
		{
			var classes = Registry.CurrentUser.CreateSubKey (@"Software\Classes");
			foreach (string extension in extensions) {
				RegistryKey key = classes.CreateSubKey (extension);
				key.SetValue ("", id);
				key.Close ();
			}

			{
				RegistryKey key = classes.CreateSubKey (id);
				key.SetValue ("", description);
				key.Close ();
			}

			if (exePath != null) {
				RegistryKey key = classes.CreateSubKey (id + @"\Shell\Open\command");
				key.SetValue ("", "\"" + exePath + "\" \"%L\"");
				key.Close ();
			}

			if (iconPath != null) {
				RegistryKey key = classes.CreateSubKey (id + @"\DefaultIcon");
				key.SetValue ("", iconPath);
				key.Close ();
			}
		}

		internal static string Windows8EmailAddress ()
		{
			string result = null;
			try {
				var exProp = Registry.CurrentUser.OpenSubKey (@"Software\Microsoft\IdentityCRL\UserExtendedProperties\");
				string[] subKeys = exProp.GetSubKeyNames ();

				if (subKeys.Length > 0) {
					result = subKeys [0];
					Log.Debug ("Windows8EmailAddress: ", subKeys.Join (", "));
				} else {
					result = null;
					Log.Debug ("Windows8EmailAddress: None");
				}
			} catch (Exception ex) {
				Log.Error ("Windows8EmailAddress: Unable to retrieve registry keys.");
				Log.Error (ex);
			}
			return result;
		}
	}
}
