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
					printNode (node: node, printAction: printNodeLongFormat);
				} else {
					printNode (node: node, printAction: printNodeShortFormat);
				}
			}

			state.ExitCode = 0;
		}

		void printNode (VirtualNode node, Action<VirtualNode> printAction)
		{
			VirtualFile file = node as VirtualFile;
			if (file != null) {
				printAction (file);
			}

			VirtualDirectory directory = node as VirtualDirectory;
			if (directory != null) {
				var list = directory.OpenList ();
				foreach (VirtualDirectory subDirectory in list.ListDirectories()) {
					printAction (subDirectory);
				}
				foreach (VirtualFile subFile in list.ListFiles()) {
					printAction (subFile);
				}
			}
		}

		void printNodeLongFormat (VirtualNode node)
		{
			// drwxrwxr-x  5 tobias tobias 4,0K Mai 21 20:39 

			string permissions, user, group, size, date, time, name;

			VirtualFile file = node as VirtualFile;
			if (file != null) {
			}

			VirtualDirectory directory = node as VirtualDirectory;
			if (directory != null) {

			}

			Output.WriteLine ($"{permissions} {user} {group} {size} {date} {time} {name}");
		}

		void printNodeShortFormat (VirtualNode node)
		{
			Output.WriteLine (node.VirtualFileName);
		}
	}
}

