using System;
using Core.Shell.Common;
using Core.Platform;
using System.IO;
using Core.Common;
using Core.IO;

namespace Core.Shell
{
	public class MainClass
	{
		public static void Main (string[] args)
		{
			Core.IO.Logging.Enable ();

			new MainClass ().Run (args);

			Core.IO.Logging.Finish ();
		}

		public void Run (string[] args)
		{
			fixFileAssociations ();

			UnixShell shell = new UnixShell ();
			shell.Output = output;

			// run code line
			if (args.Length == 2 && args [0] == "-c" && !string.IsNullOrWhiteSpace (args [1])) {
				try {
					shell.RunScript (code: args [1]);
				} catch (Exception ex) {
					Log.Error (ex);
				}
			}
			// run script
			if (args.Length == 1 && !string.IsNullOrWhiteSpace (args [1])) {
				try {
					shell.RunScript (code: File.ReadAllText (args [1]));
				} catch (Exception ex) {
					Log.Error (ex);
				}
			}
			// run test code
			else if (!SystemInfo.IsInteractive) {//  if (Console.In.Peek () == -1) {
				test (shell);
			}
			// run interactively
			else {
				string line;
				while (NonBlockingConsole.IsInputOpen) {
					while (NonBlockingConsole.TryReadLine (result: out line)) {
						if (!string.IsNullOrWhiteSpace (line)) {
							shell.Interactive (line: line);
						}
					}
				}
			}
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
			FileAssociation.SetAssociation (extensions: new[]{ ".sh", ".coresh", ".bash" }, description: "Shell Script", exePath: SystemInfo.ApplicationPath, iconPath: null);
		}

		void output (string text)
		{
			Console.Write (text);
		}
	}
}
