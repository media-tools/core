using System;
using System.Collections.Generic;
using Core.Common;
using Core.IO;

namespace Core.Shell.Platform.Commands
{
	public class PathIndex
	{
		readonly List<string> PathDirectories = new List<string> ();
		readonly Dictionary<string, NativeExecutable> PathExecutables = new Dictionary<string, NativeExecutable> ();
		readonly object lockIndex = new object ();
		readonly IExecutablePathValidator ExecutablePathValidator;
		bool IsIndexed = false;

		public PathIndex (IExecutablePathValidator executablePathValidator)
		{
			ExecutablePathValidator = executablePathValidator;
		}

		public void AddPathVariable (string pathVariable)
		{
			Log.Debug ("PathVariable: ", pathVariable);
			Log.Indent++;
			if (!string.IsNullOrWhiteSpace (pathVariable)) {
				foreach (string directory in pathVariable.Split(new []{';',':'},StringSplitOptions.RemoveEmptyEntries)) {
					addPathDirectory (directory);
				}
			}
			Log.Indent--;
		}

		void addPathDirectory (string directory)
		{
			if (!PathDirectories.Contains (directory)) {
				PathDirectories.Add (directory);
			}
		}

		void IndexExecutables ()
		{
			if (!IsIndexed) {
				lock (lockIndex) {
					Log.Debug ("IndexExecutables:");
					Log.Indent++;
					foreach (string directory in PathDirectories) {
						Log.Debug ("PathDirectory: ", directory);
						Log.Indent++;
						try {
							IEnumerable<string> files = SafeDirectoryEnumerator.EnumerateFiles (directory, "*", System.IO.SearchOption.TopDirectoryOnly);
							foreach (string fullPath in files) {
								if (ExecutablePathValidator.IsValidExecutable (fullPath: fullPath)) {
									NativeExecutable executable = new NativeExecutable (fullPath: fullPath);
									Log.Debug ("executable: ", executable);
									foreach (string commandName in executable.CommandNames) {
										if (!PathExecutables.ContainsKey (commandName)) {
											PathExecutables [commandName] = executable;
										}
									}
								}
							}
						} catch (Exception ex) {
							Log.Error (ex);
						}
						Log.Indent--;
					}
					Log.Indent--;
				}

				IsIndexed = true;
			}
		}

		public bool ContainsCommand (string commandName)
		{
			IndexExecutables ();
			return PathExecutables.ContainsKey (commandName);
		}

		public NativeExecutable GetCommand (string commandName)
		{
			IndexExecutables ();
			if (PathExecutables.ContainsKey (commandName)) {
				return PathExecutables [commandName];
			} else {
				return null;
			}
		}
	}
}

