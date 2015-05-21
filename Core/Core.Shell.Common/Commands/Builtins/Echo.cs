using System;
using System.Linq;

namespace Core.Shell.Common.Commands.Builtins
{
	public class Echo : BuiltinCommand
	{
		bool printNewline;

		public Echo ()
		{
			ExecutableName = "echo";
			UseOptions = true;

			optionSet.Add ("n", "No newline.", option => printNewline = option == null);
		}

		protected override void ResetInternalState ()
		{
			printNewline = true;
		}

		protected override void ExecuteInternal ()
		{
			Output.Write (string.Join (" ", parameters.Select (p => p ?? "null")));
			if (printNewline) {
				Output.WriteLine ();
			}
			state.ExitCode = 0;
		}
	}
}

