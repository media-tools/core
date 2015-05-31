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

		public InputHistory History { get; } = new InputHistory();

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
		public string Prompt ()
		{
			string mail = PlatformInfo.User.UserShortName ?? PlatformInfo.User.UserShortName;
			string wd = executer.Environment.WorkingDirectory.Path.FullPath ();
			string smiley = executer.Environment.StackTrace.Last ().State.IsExitSuccess
					? string.Empty //char.ConvertFromUtf32 (0x1F603) // SMILING FACE WITH OPEN MOUTH
					: char.ConvertFromUtf32 (0x1F627) + " "; // ANGUISHED FACE
			//string ifLinux = SystemInfo.OperatingSystem == ModernOperatingSystem.Linux ? char.ConvertFromUtf32 (0x1F427) : string.Empty;

			string prompt = $"{mail} {wd} {smiley}$ ";
			return prompt;
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

			GC.Collect ();
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
			string ifLinux = PlatformInfo.System.OperatingSystem == ModernOperatingSystem.Linux ? string.Format ("({0})", char.ConvertFromUtf32 (0x1F427)) : string.Empty;
			string mail = PlatformInfo.User.UserMail ?? "(only supported on Windows 8 or newer)";
			string smiley = PlatformInfo.System.OperatingSystem == ModernOperatingSystem.Linux ? char.ConvertFromUtf32 (0x1F603) : "\u003A";

			System.IO.TextWriter output = Environment.Output.ToTextWriter ();

			output.WriteLine ();
			output.WriteLine (@"  +++++++++++++++++++++++++ System Info: +++++++++++++++++++++++++");
			output.WriteLine (@"  +  ");
			output.WriteLine (@"  +  Host Name          =  " + PlatformInfo.User.HostName);
			output.WriteLine (@"  +  Operating System   =  " + PlatformInfo.System.OperatingSystem + " " + ifLinux);
			output.WriteLine (@"  +  Short User Name    =  " + PlatformInfo.User.UserShortName);
			output.WriteLine (@"  +  Full User Name     =  " + PlatformInfo.User.UserFullName);
			output.WriteLine (@"  +  Email Address      =  " + mail);
			output.WriteLine (@"  +  ");
			output.WriteLine (@"  ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
			output.WriteLine ();
			output.WriteLine (@"  Welcome to {0}, {1}! {2}", PlatformInfo.User.HostName, PlatformInfo.User.UserFullName, smiley);
			output.WriteLine ();
		}
	}
}

