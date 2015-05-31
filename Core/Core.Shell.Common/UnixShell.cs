using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Common;
using Core.IO;
using Core.Portable;
using Core.IO.Streams;

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

		public async Task InteractiveAsync (string line)
		{
			parser.Clear ();

			parser.Parse (line);

			Log.Debug ("Json:");
			Log.Indent++;
			Log.Debug (parser.Result.ToJson ());
			Log.Indent--;

			await executer.Execute (parser.Result);

			GC.Collect ();
		}

		public async Task RunScriptAsync (string code)
		{
			parser.Parse (code);

			Log.Debug ("Json:");
			Log.Indent++;
			Log.Debug (parser.Result.ToJson ());
			Log.Indent--;

			await executer.Execute (parser.Result);
		}

		public async Task PrintWelcomeAsync ()
		{
			string ifLinux = PlatformInfo.System.OperatingSystem == ModernOperatingSystem.Linux ? string.Format ("({0})", char.ConvertFromUtf32 (0x1F427)) : string.Empty;
			string mail = PlatformInfo.User.UserMail ?? "(only supported on Windows 8 or newer)";
			string smiley = PlatformInfo.System.OperatingSystem == ModernOperatingSystem.Linux ? char.ConvertFromUtf32 (0x1F603) : "\u003A";

			var output = Environment.Output;

			await output.WriteLineAsync ();
			await output.WriteLineAsync (@"  +++++++++++++++++++++++++ System Info: +++++++++++++++++++++++++");
			await output.WriteLineAsync (@"  +  ");
			await output.WriteLineAsync (@"  +  Host Name          =  " + PlatformInfo.User.HostName);
			await output.WriteLineAsync (@"  +  Operating System   =  " + PlatformInfo.System.OperatingSystem + " " + ifLinux);
			await output.WriteLineAsync (@"  +  Short User Name    =  " + PlatformInfo.User.UserShortName);
			await output.WriteLineAsync (@"  +  Full User Name     =  " + PlatformInfo.User.UserFullName);
			await output.WriteLineAsync (@"  +  Email Address      =  " + mail);
			await output.WriteLineAsync (@"  +  ");
			await output.WriteLineAsync (@"  ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
			await output.WriteLineAsync ();
			await output.WriteLineAsync (string.Format (@"  Welcome to {0}, {1}! {2}", PlatformInfo.User.HostName, PlatformInfo.User.UserFullName, smiley));
			await output.WriteLineAsync ();
		}
	}
}

