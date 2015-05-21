using System;
using Core.Platform.Windows;
using Core.Portable;

namespace Core.Platform
{
	public static class FileAssociation
	{
		public static void SetAssociation (string[] extensions, string id, string description, string exePath, string iconPath)
		{
			if (SystemInfo.OperatingSystem == ModernOperatingSystem.WindowsDesktop) {
				WindowsRegistry.SetAssociation (extensions: extensions, id: id, description: description, exePath: exePath, iconPath: iconPath);
			}
		}
	}

}

