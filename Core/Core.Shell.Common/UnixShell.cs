using System;
using Core.Common;
using Core.IO;
using Core.Portable;
using System.Linq;
using Core.Shell.Common.FileSystems;

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

		//
		// Smileys:
		// char.ConvertFromUtf32 (0x1F603) // SMILING FACE WITH OPEN MOUTH
		// char.ConvertFromUtf32 (0x1F627) + " "; // ANGUISHED FACE
		//
		public string Prompt {
			get {
				string user = UserInfo.FullNameAndMail;
				string mail = UserInfo.UserMail ?? UserInfo.UserAtHostName;
				VirtualDirectory wd = executer.Environment.WorkingDirectory;
				string smiley = executer.Environment.StackTrace.Last ().State.IsExitSuccess
					? string.Empty //char.ConvertFromUtf32 (0x1F603) // SMILING FACE WITH OPEN MOUTH
					: char.ConvertFromUtf32 (0x1F627) + " "; // ANGUISHED FACE
				//string ifLinux = SystemInfo.OperatingSystem == ModernOperatingSystem.Linux ? char.ConvertFromUtf32 (0x1F427) : string.Empty;
				
				string prompt = $"{mail} {wd} {smiley}$ ";
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

		public void PrintWelcome ()
		{
			string ifLinux = SystemInfo.OperatingSystem == ModernOperatingSystem.Linux ? string.Format ("({0})", char.ConvertFromUtf32 (0x1F427)) : string.Empty;

			Environment.Output.WriteLine ();
			Environment.Output.WriteLine (@"  +++++++++++++++++++++++++ System Info: +++++++++++++++++++++++++");
			Environment.Output.WriteLine (@"  +  ");
			Environment.Output.WriteLine (@"  +  Host Name          =  " + UserInfo.HostName);
			Environment.Output.WriteLine (@"  +  Operating System   =  " + SystemInfo.OperatingSystem + " " + ifLinux);
			Environment.Output.WriteLine (@"  +  Short User Name    =  " + UserInfo.UserShortName);
			Environment.Output.WriteLine (@"  +  Full User Name     =  " + UserInfo.UserFullName);
			Environment.Output.WriteLine (@"  +  Email Address      =  " + UserInfo.UserMail);
			Environment.Output.WriteLine (@"  +  ");
			Environment.Output.WriteLine (@"  ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
			Environment.Output.WriteLine ();
			Environment.Output.WriteLine (@"  Welcome to {0}, {1}! {2}", UserInfo.HostName, UserInfo.UserShortName, char.ConvertFromUtf32 (0x1F603));
			Environment.Output.WriteLine ();
		}
	}
}

