using System;
using System.Collections.Generic;
using System.IO;
using Core.Common;
using Core.IO;
using Core.Shell.Common.Commands;
using System.Diagnostics;

namespace Core.Shell.Platform.Commands
{
	public class NativeExecutableCommand : AbstractCommand
	{
		NativeExecutable Executable;

		public NativeExecutableCommand (NativeExecutable executable)
		{
			Executable = executable;
			ExecutableName = executable.FileName;
		}

		protected override void ResetInternalState ()
		{
		}

		protected override void ExecuteInternal ()
		{
			try {
				ProcessStartInfo startInfo = new ProcessStartInfo {
					FileName = Executable.FullPath,
					Arguments = ArgumentString (),
					UseShellExecute = false,
					CreateNoWindow = true,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					StandardOutputEncoding = System.Text.Encoding.UTF8,
					StandardErrorEncoding = System.Text.Encoding.UTF8,
				};

				Process process = new Process {
					StartInfo = startInfo,
				};

				process.OutputDataReceived += (sender, args) => {
					if (args.Data != null) {
						Log.Debug ("received output: ", args.Data);
						Output.WriteLine (args.Data);
					}
				};
				process.ErrorDataReceived += (sender, args) => {
					if (args.Data != null) {
						Log.Debug ("received error: ", args.Data);
						Error.WriteLine (args.Data);
					}
				};

				process.Start ();
				process.BeginOutputReadLine ();
				process.BeginErrorReadLine ();
				process.WaitForExit ();

				state.ExitCode = process.ExitCode;

			} catch (Exception ex) {
				Error.WriteLine (ex);
				state.ExitCode = 1;
			}
		}

		string ArgumentString ()
		{
			List<string> escapedArgumentList = new List<string> ();
			foreach (string arg in parameters) {
				string escapedArgument = null;
				if (arg.Contains ("\"") && !arg.Contains ("'")) {
					escapedArgument = $"'{arg}'";
				} else if (!arg.Contains ("\"") && arg.Contains ("'")) {
					escapedArgument = $"\"{arg}\"";
				} else if (arg.Contains ("\"") && arg.Contains ("'")) {
					escapedArgument = arg;
				} else {
					escapedArgument = $"\"{arg}\"";
				}
				escapedArgumentList.Add (escapedArgument);
			}
			return escapedArgumentList.Join (" ");
		}
	}
}

