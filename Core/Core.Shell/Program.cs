using System;
using System.Collections.Generic;
using System.IO;
using Core.Common;
using Core.IO;
using Core.Platform;
using Core.Portable;
using Core.Shell.Common;
using Core.Shell.Platform.FileSystems;
using Mono.Options;

namespace Core.Shell
{
	public class MainClass
	{
		public static void Main (string[] args)
		{
			Mono.Unix.FileAccessPermissions fuck = Mono.Unix.FileAccessPermissions.GroupExecute;
			Console.WriteLine (fuck);

			DesktopPlatform.Start ();

			new MainClass ().Run (args);

			DesktopPlatform.Finish ();
		}

		public void Run (string[] args)
		{
			fixFileAssociations ();
			
			DesktopPlatform.LogTargets.StandardOutput = false;

			// option values
			bool help = false;
			Mode mode = Mode.Interactive;
			string commandString = null;
			string scriptFile = null;
			List<string> parameters = new List<string> ();

			// option parser
			OptionSet optionSet = new OptionSet ();
			optionSet.Add ("h|help|?", "Prints out the options.", option => help = (option != null));
			optionSet.Add ("log|debug", "Show log messages.", option => DesktopPlatform.LogTargets.StandardOutput = option != null);
			optionSet.Add ("c=", "Execute a command string.", option => {
				mode = Mode.CommandString;
				commandString = option;
			});
			optionSet.Add ("<>", option => {
				if (mode != Mode.CommandString && scriptFile == null) {
					mode = Mode.ScriptFile;
					scriptFile = option;
					Log.Warning ("Script file: ", option);
				} else {
					parameters.Add (option);
					Log.Warning ("Parameter: ", option);
				}
			});
			try {
				optionSet.Parse (args);
			} catch (OptionException) {
				help = true;
			}

			// need help ?
			if (help) {
				printOptions (optionSet);
				return;
			}

			RegularFileSystems.Register ();
			UnixShell shell = new UnixShell ();
			shell.Environment.Output.Stream = output;
			shell.Environment.Error.Stream = output;

			// run code line
			if (mode == Mode.CommandString) {
				try {
					shell.RunScript (code: commandString);
				} catch (Exception ex) {
					Log.Error (ex);
				}
			}
			// run script
			if (mode == Mode.ScriptFile) {
				try {
					shell.RunScript (code: File.ReadAllText (scriptFile));
				} catch (Exception ex) {
					Log.Error (ex);
				}
			}
			// run interactively
			if (mode == Mode.Interactive) {
				
				// run test code if there is no interactive console
				if (!PlatformInfo.System.IsInteractive) {
					DesktopPlatform.LogTargets.StandardOutput = true;
					test (shell);
				}

				// run interactively
				else {
					shell.PrintWelcome ();
					string line;
					NonBlockingConsole.Write (shell.Prompt);
					while (NonBlockingConsole.IsInputOpen) {
						while (NonBlockingConsole.TryReadLine (result: out line)) {
							if (!string.IsNullOrWhiteSpace (line)) {
								shell.Interactive (line: line);
							}
							NonBlockingConsole.Write (shell.Prompt);
						}
					}
					NonBlockingConsole.WriteLine (string.Empty);
				}
			}
		}

		private void printOptions (OptionSet optionSet)
		{
			optionSet.WriteOptionDescriptions (Console.Out);
		}

		public enum Mode
		{
			Interactive,
			CommandString,
			ScriptFile,
		}

		void test (UnixShell shell)
		{
			shell.Interactive (@"
				echo test;echo test2
				echo test7 a  ""b c d"" """" ''      'c' abc;
				if true;
				then
					echo test4;
				elif false ; then
					echo test5
				else ;;;
					echo test6; echo test7
				fi
			");
		}

		void fixFileAssociations ()
		{
			FileAssociation.SetAssociation (
				extensions: new[]{ ".sh", ".coresh", ".bash" },
				id: "ShellScript",
				description: "Shell Script",
				exePath: PlatformInfo.System.ApplicationPath,
				iconPath: Path.Combine (Path.GetDirectoryName (PlatformInfo.System.ApplicationPath), "icon.ico")
			);
		}

		void output (string text)
		{
			NonBlockingConsole.Write (text);
		}
	}
}
