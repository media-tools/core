using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common;
using Mono.Options;

namespace Core.Shell.Common.Commands
{
	public abstract class CommandSubsystem
	{
		public abstract bool ContainsCommand (string commandName);

		public abstract ICommand GetCommand (string commandName);
	}
}

