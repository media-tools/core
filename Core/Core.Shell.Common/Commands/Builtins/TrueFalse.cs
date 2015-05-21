using System;

namespace Core.Shell.Common.Commands.Builtins
{
	public class True : BuiltinCommand
	{
		public True ()
		{
			ExecutableName = "true";
		}

		protected override void ExecuteInternal ()
		{
			state.ExitCode = 0;
		}
	}

	public class False : BuiltinCommand
	{
		public False ()
		{
			ExecutableName = "false";
		}

		protected override void ExecuteInternal ()
		{
			state.ExitCode = 1;
		}
	}
}

