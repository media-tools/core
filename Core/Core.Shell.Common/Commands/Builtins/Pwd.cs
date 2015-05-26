using System;
using System.Linq;
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

		protected override void ExecuteInternal ()
		{
			Output.WriteLine (env.WorkingDirectory.Path.FullPath ());
		}
	}
}

