using System;
using System.Collections.Generic;
using System.IO;
using Core.Common;
using Core.IO;
using Core.Shell.Common.Commands;
using System.Diagnostics;
using System.Threading.Tasks;

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

		protected override async Task ExecuteInternalAsync ()
		{
			Process process = null;
			try {
				process = new Process { };
				
				process.StartInfo = new ProcessStartInfo {
					FileName = Executable.FullPath,
					Arguments = ArgumentString (),
					UseShellExecute = false,
					CreateNoWindow = true,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					RedirectStandardInput = true,
					StandardOutputEncoding = System.Text.Encoding.UTF8,
					StandardErrorEncoding = System.Text.Encoding.UTF8,
				};
				process.EnableRaisingEvents = true;

				List<Task> waitfor = new List<Task> ();

				var tcs = new TaskCompletionSource<object> ();
				waitfor.Add (tcs.Task);
				process.Exited += (sender, args) => {
					Log.Debug (GetType ().Name, ": Process Exited!");
					tcs.SetResult (null);
				};

				Log.Debug (GetType ().Name, ": process.Start ()");
				process.Start ();
				//process.BeginOutputReadLine ();
				//process.BeginErrorReadLine ();
				//process.WaitForExit ();

				waitfor.Add (Task.Run (async () => await Output.Eat (process.StandardOutput)));
				waitfor.Add (Task.Run (async () => await Error.Eat (process.StandardError)));
				Input.PipeTo (process.StandardInput);

				await Task.Run (() => {
					Log.Debug ("awaiting all...");
					Task.WaitAll (waitfor.ToArray ());
					Input.PipeToCache ();
					Log.Debug ("all done!");
					state.ExitCode = process.ExitCode;
					process.Dispose ();
				});

			} catch (Exception ex) {
				Log.Debug (GetType ().Name, ": Error!");
				Log.Error (ex);
				Error.ToTextWriter ().WriteLine (ex);
				state.ExitCode = 1;
				if (process != null) {
					process.Dispose ();
				}
				//	tcs.SetException (new InvalidOperationException ("The process did not exit correctly. " + "The corresponding error message was: " + errorMessage));
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

