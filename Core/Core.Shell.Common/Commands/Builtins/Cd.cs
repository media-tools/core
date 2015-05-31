using System;
using System.Threading.Tasks;
using Core.Common;
using Core.Shell.Common.FileSystems;

namespace Core.Shell.Common.Commands.Builtins
{
	public class Cd : BuiltinCommand
	{
		public Cd ()
		{
			ExecutableName = "cd";
			UseOptions = false;
		}

		protected override void ResetInternalState ()
		{
		}

		protected override Task ExecuteInternalAsync ()
		{
			VirtualDirectory node =
				parameters.Count >= 1
				? FileSystemSubsystems.Directory (FileSystemSubsystems.ResolveRelativePath (env) (parameters [0]))
				: env.HomeDirectory;
			Log.Debug ("change directory to: ", node);

			node.Validate (throwExceptions: true);

			env.WorkingDirectory = node;

			return TaskHelper.Completed;
		}
	}
}

