using System;
using Microsoft.Win32;

namespace Core.Platform
{
	public static class FileAssociation
	{
		public static void SetAssociation (string[] extensions, string id, string description, string exePath, string iconPath)
		{
			if (SystemInfo.IsRunningOnWindows ()) {
				WindowsImpl.setAssociation (extensions: extensions, id: id, description: description, exePath: exePath, iconPath: iconPath);
			}
		}

		static class WindowsImpl
		{
			internal static void setAssociation (string[] extensions, string id, string description, string exePath, string iconPath)
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
		}
	}
}

