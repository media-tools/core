using System;
using Microsoft.Win32;
using Core.Platform.Windows;

namespace Core.Platform
{
	public static class FileAssociation
	{
		public static void SetAssociation (string[] extensions, string id, string description, string exePath, string iconPath)
		{
			if (SystemInfo.IsRunningOnWindows ()) {
				WindowsRegistry.SetAssociation (extensions: extensions, id: id, description: description, exePath: exePath, iconPath: iconPath);
			}
		}
	}

}

