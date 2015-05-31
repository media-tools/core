using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Common;
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

		protected override async Task ExecuteInternalAsync ()
		{
			useLongFormat |= invokedExecutableName.StartsWith ("ll");

			Log.Debug ("parameters: ", parameters.Join (", "));
			List<VirtualNode> nodes = await resolveParameters ();
			Log.Debug ("nodes: ", nodes.Join (", "));

			// print the resolved nodes
			foreach (VirtualNode node in nodes) {
				if (nodes.Count > 1) {
					await Output.WriteLineAsync (node + ":");
				}

				try {
					if (useLongFormat) {
						await printNode (node: node, printAction: printNodeLongFormat);
					} else {
						await printNode (node: node, printAction: printNodeShortFormat);
					}
				} catch (VirtualIOException ex) {
					Log.Error (ex);
					await Output.WriteLineAsync (ex.Message);
				}

				if (nodes.Count > 1) {
					await Output.WriteLineAsync ();
				}
			}

			state.ExitCode = 0;
		}

		async Task<List<VirtualNode>> resolveParameters ()
		{
			List<VirtualNode> nodes = new List<VirtualNode> ();

			// if there are command line parameters, resolve them
			if (parameters.Count > 0) {
				foreach (string p in parameters) {
					try {
						Path resolvedPath = FileSystemSubsystems.ResolveRelativePath (env) (p);
						VirtualNode node = FileSystemSubsystems.Node (resolvedPath);
						nodes.Add (node);
					} catch (VirtualIOException ex) {
						Log.Error (ex);
						await Output.WriteLineAsync (ex.Message);
					}
				}
			}
			// otherwise, use the current working directory
			else {
				nodes.Add (env.WorkingDirectory);
			}

			return nodes;
		}

		async Task printNode (VirtualNode node, AsyncAction<VirtualNode> printAction)
		{
			VirtualFile file = node as VirtualFile;
			if (file != null) {
				await printAction (file);
			}

			VirtualDirectory directory = node as VirtualDirectory;
			if (directory != null) {
				var list = directory.OpenList ();
				foreach (VirtualDirectory subDirectory in list.ListDirectories()) {
					await printAction (subDirectory);
				}
				foreach (VirtualFile subFile in list.ListFiles()) {
					await printAction (subFile);
				}
			}
		}

		async Task printNodeLongFormat (VirtualNode node)
		{
			// drwxrwxr-x  5 tobias tobias 4,0K Mai 21 20:39 

			string type, permissions, user, group, size, date, time, name;

			permissions = node.PermissionsString;
			name = node.Path.FileName;
			user = node.OwnerName;
			group = node.OwnerName;

			VirtualFile file = node as VirtualFile;
			if (file != null) {
			}

			VirtualDirectory directory = node as VirtualDirectory;
			if (directory != null) {
			}

			await Output.WriteLineAsync ($"{permissions} {user} {group} {size} {date} {time} {name}");
		}

		async Task printNodeShortFormat (VirtualNode node)
		{
			await Output.WriteLineAsync (node.Path.FileName);
		}
	}
}

