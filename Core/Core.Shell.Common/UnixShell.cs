using System;
using Core.Common;
using Core.IO;
using Core.Portable;

namespace Core.Shell.Common
{
	public class UnixShell
	{
		public Action<string> Output = s => new ArgumentException ("UnixShell.Output must not be null!").ThrowAction<string> ();

		readonly Parser parser = new Parser ();
		readonly Executer executer = new Executer ();

		public UnixShell ()
		{
		}

		public string Prompt {
			get {
				string user = UserInfo.FullNameAndMail;
				string mail = UserInfo.UserMail ?? UserInfo.UserAtHostName;
				string wd = executer.State.WorkingDirectory;
				//return $"┌┼───┤ {user} ├───┤ 23:35:27 ├────────────┤ {wd} ├──── \n"	+ "└┼─$─┤► ";
				string prompt = $"{mail} {wd} $ ";
				return prompt;
			}
		}

		public void Interactive (string line)
		{
			parser.Clear ();

			parser.Parse (line);

			Log.Debug ("Json:");
			Log.Indent++;
			Log.Debug (parser.Result.ToJson ());
			Log.Indent--;

			executer.Execute (parser.Result);

			//Output (line);
		}

		public void RunScript (string code)
		{
			parser.Parse (code);

			Log.Debug ("Json:");
			Log.Indent++;
			Log.Debug (parser.Result.ToJson ());
			Log.Indent--;

			executer.Execute (parser.Result);
		}
	}
}

