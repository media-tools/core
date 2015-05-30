using System;
using System.Collections.Generic;
using System.IO;
using Core.Common;
using Core.IO;
using Core.Shell.Common.Commands;

namespace Core.Shell.Platform.Commands
{
	public abstract class RegularExecutableSubsystem : CommandSubsystem
	{
		protected List<string> PathDirectories = new List<string> ();
		protected Dictionary<string, NativeExecutable> PathExecutables = new Dictionary<string, NativeExecutable> ();
		bool IsIndexed = false;
		object lockIndex = new object ();

		protected RegularExecutableSubsystem ()
		{
			addPathVariable (Environment.GetEnvironmentVariable ("PATH", EnvironmentVariableTarget.Machine));
			addPathVariable (Environment.GetEnvironmentVariable ("PATH", EnvironmentVariableTarget.User));
			addPathVariable (Environment.GetEnvironmentVariable ("PATH", EnvironmentVariableTarget.Process));
		}

		void addPathVariable (string pathVariable)
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
						try {
							IEnumerable<string> files = SafeDirectoryEnumerator.EnumerateFiles (directory, "*", SearchOption.TopDirectoryOnly);
							foreach (string fullPath in files) {
								if (IsValidExutable (fullPath: fullPath)) {
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
					}
					Log.Indent--;
				}

				IsIndexed = true;
			}
		}

		protected abstract bool IsValidExutable (string fullPath);

		public override bool ContainsCommand (string commandName)
		{
			IndexExecutables ();
			return PathExecutables.ContainsKey (commandName);
		}

		public override ICommand GetCommand (string commandName)
		{
			IndexExecutables ();
			if (PathExecutables.ContainsKey (commandName)) {
				return new NativeExecutableCommand (PathExecutables [commandName]);
			} else {
				return null;
			}
		}
	}

}

