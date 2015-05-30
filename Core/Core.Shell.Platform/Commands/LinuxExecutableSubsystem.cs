using System;
using Core.IO;
using Core.Portable;
using Core.Shell.Common.Commands;

namespace Core.Shell.Platform.Commands
{
	public sealed class LinuxExecutableSubsystem : RegularExecutableSubsystem
	{
		protected override bool IsValidExutable (string fullPath)
		{
			return FileHelper.Instance.CanExecute (fullPath);
		}
	}

}

