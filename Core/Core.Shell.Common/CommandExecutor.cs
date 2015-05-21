using System;
using Core.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Shell.Common.Commands;

namespace Core.Shell.Common
{
	public class CommandExecutor
	{
		public string Executable { get; private set; }

		public string[] Params { get; private set; }

		public CommandExecutor (ICommandBlock block)
		{
			string cmd = block.ContentString;
			string[] p = parameterize (cmd).Where (s => s != null).ToArray ();
			Executable = p [0];
			Params = p.Skip (1).ToArray ();
			Log.Debug ("Params: ", Params.Join (" "));
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

		public void Execute (ref ExecutionEnvironment env)
		{
			Log.Warning ("Executable: ", Executable);
			Log.Debug ("Params: ", Params.ToJson ());

			if (CommandSubsystems.ContainsCommand (commandName: Executable)) {
				Log.Debug ("Command is valid.");
				ICommand command = CommandSubsystems.GetCommand (commandName: Executable);
				command.Execute (parameters: Params, env: ref env);
			} else {
				Log.Error ("No such command!");
			}
		}
	}
}

