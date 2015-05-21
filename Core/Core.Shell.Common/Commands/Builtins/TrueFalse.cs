using System;

namespace Core.Shell.Common.Commands.Builtins
{
	public class True : BuiltinCommand
	{
		public override string ExecutableName { get { return "true"; } }

		protected override void ExecuteInternal ()
		{
			state.ExitCode = 0;
		}
	}

	public class False : BuiltinCommand
	{
		public override string ExecutableName { get { return "false"; } }

		protected override void ExecuteInternal ()
		{
			state.ExitCode = 1;
		}
	}
}

