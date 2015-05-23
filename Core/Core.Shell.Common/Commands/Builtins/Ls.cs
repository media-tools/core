using System;
using System.Linq;
using Core.Shell.Common.FileSystems;

namespace Core.Shell.Common.Commands.Builtins
{
	public class Ls : BuiltinCommand
	{
		bool useLongFormat;
		bool useHumanReadableSizes;

		public Ls ()
		{
			ExecutableName = "ls";
			UseOptions = true;

			optionSet.Add ("l", "use a long listing format", option => useLongFormat = option == null);
			optionSet.Add ("h", "with -l and/or -s, print human readable sizes (e.g., 1K 234M 2G)", option => useHumanReadableSizes = option == null);
		}

		protected override void ResetInternalState ()
		{
			useLongFormat = false;
			useHumanReadableSizes = false;
		}

		protected override void ExecuteInternal ()
		{
			useLongFormat |= invokedExecutableName == "ll";

			VirtualNode[] nodes = parameters.Select (FileSystemSubsystems.Node).ToArray ();
			if (nodes.Length == 0) {
				nodes = new [] { env.WorkingDirectory };
			}

			foreach (VirtualNode node in nodes) {
				if (useLongFormat) {
					printLongFormat (node);
				} else {
					printShortFormat (node);
				}
			}

			state.ExitCode = 0;
		}

		void printLongFormat (VirtualNode node)
		{
			throw new NotImplementedException ();
		}

		void printShortFormat (VirtualNode node)
		{
			VirtualFile file = node as VirtualFile;
			if (file != null) {
				Output.WriteLine (file);
			}

			VirtualDirectory directory = node as VirtualDirectory;
			var list = directory.OpenList ();
			if (directory != null) {
				foreach (VirtualDirectory subDirectory in list.ListDirectories()) {
					Output.WriteLine (subDirectory);
				}
				foreach (VirtualFile subFile in list.ListFiles()) {
					Output.WriteLine (subFile);
				}
			}
		}
	}
}

