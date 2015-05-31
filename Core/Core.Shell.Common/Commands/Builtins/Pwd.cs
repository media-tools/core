using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Portable;

namespace Core.Shell.Common.Commands.Builtins
{
	public class Pwd : BuiltinCommand
	{
		public Pwd ()
		{
			ExecutableName = "pwd";
		}

		protected override void ResetInternalState ()
		{
		}

		protected override async Task ExecuteInternalAsync ()
		{
			await Output.WriteLineAsync (env.WorkingDirectory.Path.FullPath ());
		}
	}
}

