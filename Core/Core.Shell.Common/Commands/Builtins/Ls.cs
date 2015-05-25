using System;
using System.Linq;
using Core.Shell.Common.FileSystems;
using Core.Common;
using System.Collections.Generic;

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
			useLongFormat |= invokedExecutableName.StartsWith ("ll");

			Log.Debug ("parameters: ", parameters.Join (", "));
			List<IVirtualNode> nodes = resolveParameters ();
			Log.Debug ("nodes: ", nodes.Join (", "));

			// print the resolved nodes
			foreach (VirtualNode node in nodes) {
				if (nodes.Count > 1) {
					Output.WriteLine (node + ":");
				}

				try {
					if (useLongFormat) {
						printNode (node: node, printAction: printNodeLongFormat);
					} else {
						printNode (node: node, printAction: printNodeShortFormat);
					}
				} catch (VirtualIOException ex) {
					Log.Error (ex);
					Output.WriteLine (ex.Message);
				}

				if (nodes.Count > 1) {
					Output.WriteLine ();
				}
			}

			state.ExitCode = 0;
		}

		List<IVirtualNode> resolveParameters ()
		{
			List<IVirtualNode> nodes = new List<IVirtualNode> ();

			// if there are command line parameters, resolve them
			if (parameters.Count > 0) {
				foreach (string p in parameters) {
					try {
						string resolvedPath = FileSystemSubsystems.ResolveRelativePath (env) (p);
						IVirtualNode node = FileSystemSubsystems.Node (resolvedPath);
						nodes.Add (node);
					} catch (VirtualIOException ex) {
						Log.Error (ex);
						Output.WriteLine (ex.Message);
					}
				}
			}
			// otherwise, use the current working directory
			else {
				nodes.Add (env.WorkingDirectory);
			}

			return nodes;
		}

		void printNode (VirtualNode node, Action<IVirtualNode> printAction)
		{
			IVirtualFile file = node as IVirtualFile;
			if (file != null) {
				printAction (file);
			}

			IVirtualDirectory directory = node as IVirtualDirectory;
			if (directory != null) {
				var list = directory.OpenList ();
				foreach (IVirtualDirectory subDirectory in list.ListDirectories()) {
					printAction (subDirectory);
				}
				foreach (IVirtualFile subFile in list.ListFiles()) {
					printAction (subFile);
				}
			}
		}

		void printNodeLongFormat (IVirtualNode node)
		{
			// drwxrwxr-x  5 tobias tobias 4,0K Mai 21 20:39 

			string type, permissions, user, group, size, date, time, name;

			permissions = node.PermissionsString;
			name = node.VirtualFileName;
			user = node.OwnerName;
			group = node.OwnerName;

			IVirtualFile file = node as IVirtualFile;
			if (file != null) {
			}

			IVirtualDirectory directory = node as IVirtualDirectory;
			if (directory != null) {
			}

			Output.WriteLine ($"{permissions} {user} {group} {size} {date} {time} {name}");
		}

		void printNodeShortFormat (IVirtualNode node)
		{
			Output.WriteLine (node.VirtualFileName);
		}
	}
}

