using System;
using Core.Shell.Common;

namespace Core.Shell
{
	public class MainClass
	{
		public static void Main (string[] args)
		{
			new MainClass ().Run ();
		}

		public void Run ()
		{
			UnixShell shell = new UnixShell ();
			shell.Output = output;

			string line;
			while ((line = Console.ReadLine ()) != null) {
				shell.Interactive (line: line);
			}
		}

		void output (string text)
		{
			Console.Write (text);
		}
	}
}
