using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common;
using Core.IO.Streams;
using Core.Shell.Common.Commands;

namespace Core.Shell.Common
{
	public class CommandExecutor
	{
		public string ExecutableName { get; private set; }

		public string[] Params { get; private set; }

		public CommandExecutor (ICommandBlock block)
		{
			string cmd = block.ContentString;
			string[] p = parameterize (cmd).Where (s => s != null).ToArray ();
			ExecutableName = p [0];
			Params = p.Skip (1).ToArray ();
		}

		IEnumerable<string> parameterize (string cmd)
		{
			Dictionary<char, bool> inside = new Dictionary<char, bool> {
				['"' ] = false,
				['\'' ] = false,
			};
			StringBuilder parameter = new StringBuilder ();
			for (int i = 0; i < cmd.Length; i++) {
				char c = cmd [i];
				if (c == '"' || c == '\'') {
					inside [c] = !inside [c];
				}
				if (char.IsWhiteSpace (c) && !inside.Values.Any (e => e)) {
					yield return formatParameter (parameter.ToString ());
					parameter.Clear ();
				} else {
					parameter.Append (c);
				}
			}
			yield return formatParameter (parameter.ToString ());
		}

		string formatParameter (string p)
		{
			if (string.IsNullOrWhiteSpace (p))
				return null;
			
			if (p.Length > 0 && p [0] == '"' && p [p.Length - 1] == '"')
				p = p.Substring (1, p.Length - 2);
			if (p.Length > 0 && p [0] == '\'' && p [p.Length - 1] == '\'')
				p = p.Substring (1, p.Length - 2);
			
			return p;
		}

		public async Task ExecuteAsync (ExecutionEnvironment env)
		{
			if (CommandSubsystems.ContainsCommand (commandName: ExecutableName)) {
				ICommand command = CommandSubsystems.GetCommand (commandName: ExecutableName);
				try {
					await command.ExecuteAsync (invokedExecutableName: ExecutableName, parameters: Params, env: env);
				} catch (Exception ex) {
					Log.Error (ex);
					await env.Output.WriteLineAsync ("Error! " + ex.Message);
				}
			} else {
				Log.Error ("Command not found: " + ExecutableName);
				await env.Error.WriteLineAsync ("Command not found: " + ExecutableName);

				StackTraceElement element = new StackTraceElement {
					Executable = ExecutableName,
					Parameters = Params,
					State = new ExecutionState {
						ExitCode = 127
					}
				};
				env.StackTrace.Add (element);
			}
		}
	}
}

