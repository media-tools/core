using System;
using System.Linq;
using System.Threading.Tasks;

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

		protected override async Task ExecuteInternalAsync ()
		{
			await Output.WriteAsync (string.Join (" ", parameters.Select (p => p ?? "null")));
			if (printNewline) {
				await Output.WriteLineAsync ();
			}
			state.ExitCode = 0;
		}
	}
}

