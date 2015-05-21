using System;
using Core.Common;
using Core.IO;
using Core.Portable;

namespace Core.Shell.Common
{
	public class UnixShell
	{
		public ExecutionEnvironment Environment { get { return executer.Environment; } }

		readonly Parser parser = new Parser ();
		readonly Executer executer = new Executer ();

		public UnixShell ()
		{
		}

		public string Prompt {
			get {
				string user = UserInfo.FullNameAndMail;
				string mail = UserInfo.UserMail ?? UserInfo.UserAtHostName;
				string wd = executer.Environment.WorkingDirectory;
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

