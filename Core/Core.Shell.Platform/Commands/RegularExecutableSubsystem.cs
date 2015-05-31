using System;
using System.Collections.Generic;
using System.IO;
using Core.Common;
using Core.IO;
using Core.Shell.Common.Commands;

namespace Core.Shell.Platform.Commands
{
	public abstract class RegularExecutableSubsystem : CommandSubsystem, IExecutablePathValidator
	{
		protected PathIndex PathIndex;

		protected RegularExecutableSubsystem ()
		{
			PathIndex = new PathIndex (executablePathValidator: this);
			PathIndex.AddPathVariable (Environment.GetEnvironmentVariable ("PATH", EnvironmentVariableTarget.Machine));
			PathIndex.AddPathVariable (Environment.GetEnvironmentVariable ("PATH", EnvironmentVariableTarget.User));
			PathIndex.AddPathVariable (Environment.GetEnvironmentVariable ("PATH", EnvironmentVariableTarget.Process));
		}

		#region IExecutablePathValidator implementation

		bool IExecutablePathValidator.IsValidExecutable (string fullPath)
		{
			return IsValidExecutable (fullPath: fullPath);
		}

		#endregion

		protected abstract bool IsValidExecutable (string fullPath);

		public override bool ContainsCommand (string commandName)
		{
			try {
				return PathIndex.ContainsCommand (commandName) || FileHelper.Instance.IsFile (commandName);
			} catch (Exception ex) {
				Log.Warning (ex);
				return false;
			}
		}

		public override ICommand GetCommand (string commandName)
		{
			try {
				NativeExecutable exe = PathIndex.GetCommand (commandName)
				                       ?? (FileHelper.Instance.IsFile (commandName) ? new NativeExecutable (commandName) : null);
				return exe != null ? new NativeExecutableCommand (executable: exe) : null;
			} catch (Exception ex) {
				Log.Warning (ex);
				return null;
			}
		}
	}

}

