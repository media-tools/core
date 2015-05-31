using System;
using System.Threading.Tasks;
using Core.Common;

namespace Core.Shell.Common.Commands.Builtins
{
	public class True : BuiltinCommand
	{
		public True ()
		{
			ExecutableName = "true";
		}

		protected override void ResetInternalState ()
		{
		}

		protected override Task ExecuteInternalAsync ()
		{
			state.ExitCode = 0;
			return TaskHelper.Completed;
		}
	}

	public class False : BuiltinCommand
	{
		public False ()
		{
			ExecutableName = "false";
		}

		protected override void ResetInternalState ()
		{
		}

		protected override Task ExecuteInternalAsync ()
		{
			state.ExitCode = 1;
			return TaskHelper.Completed;
		}
	}
}

