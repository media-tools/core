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
			return PathIndex.ContainsCommand (commandName);
		}

		public override ICommand GetCommand (string commandName)
		{
			NativeExecutable exe = PathIndex.GetCommand (commandName);
			return exe != null ? new NativeExecutableCommand (executable: exe) : null;
		}
	}

}

