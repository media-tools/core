using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common;
using Mono.Options;

namespace Core.Shell.Common.Commands
{
	public static class CommandSubsystems
	{
		public static List<CommandSubsystem> Subsystems = new List<CommandSubsystem> {
			new BuiltinCommandSubsystem (),
		};

		public static bool ContainsCommand (string commandName)
		{
			foreach (CommandSubsystem ss in Subsystems) {
				if (ss.ContainsCommand (commandName)) {
					return true;
				}
			}
			return false;
		}

		public static ICommand GetCommand (string commandName)
		{
			foreach (CommandSubsystem ss in Subsystems) {
				if (ss.ContainsCommand (commandName)) {
					return ss.GetCommand (commandName);
				}
			}
			return null;
		}
	}
	
}
