using System;
using System.Collections.Generic;

namespace Core.Shell.Common.Commands
{
	public class BuiltinCommandSubsystem : CommandSubsystem
	{
		public Dictionary<string, BuiltinCommand> Commands = new Dictionary<string, BuiltinCommand> {
			["true" ] = new Builtins.True (),
			["false" ] = new Builtins.False (),
			["echo" ] = new Builtins.Echo (),
			["sleep" ] = new Builtins.Sleep (),
			["cd" ] = new Builtins.Cd (),
			["ls" ] = new Builtins.Ls (),
			["ll" ] = new Builtins.Ls (),
			["l" ] = new Builtins.Ls (),
			["lla" ] = new Builtins.Ls (),
			["llas" ] = new Builtins.Ls (),
		};

		public override bool ContainsCommand (string commandName)
		{
			return Commands.ContainsKey (commandName);
		}

		public override ICommand GetCommand (string commandName)
		{
			return Commands.ContainsKey (commandName) ? Commands [commandName] : null;
		}
	}
}

