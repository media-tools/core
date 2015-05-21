using System;
using System.Linq;

namespace Core.Shell.Common.Commands.Builtins
{
	public class Echo : BuiltinCommand
	{
		public override string ExecutableName { get { return "echo"; } }

		protected override void ExecuteInternal ()
		{
			Output (string.Join (" ", parameters.Select (p => p ?? "null")));
			state.ExitCode = 0;
		}
	}
}

