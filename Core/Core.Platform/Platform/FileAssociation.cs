using System;
using Microsoft.Win32;

namespace Core.Platform
{
	public static class FileAssociation
	{
		public static void SetAssociation (string[] extensions, string description, string exePath, string iconPath)
		{
			if (SystemInfo.IsRunningOnWindows ()) {
				WindowsImpl.setAssociation (extensions: extensions, description: description, exePath: exePath, iconPath: iconPath);
			}
		}

		static class WindowsImpl
		{
			internal static void setAssociation (string[] extensions, string description, string exePath, string iconPath)
			{
				foreach (string extension in extensions) {
					RegistryKey key = Registry.ClassesRoot.CreateSubKey (extension);
					key.SetValue ("", description);
					key.Close ();
				}

				if (exePath != null) {
					RegistryKey key = Registry.ClassesRoot.CreateSubKey (description + @"\Shell\Open\command");
					key.SetValue ("", "\"" + exePath + "\" \"%L\"");
					key.Close ();
				}

				if (iconPath != null) {
					RegistryKey key = Registry.ClassesRoot.CreateSubKey (description + @"\DefaultIcon");
					key.SetValue ("", iconPath);
					key.Close ();
				}
			}
		}
	}
}

