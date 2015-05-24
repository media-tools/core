using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common;
using Mono.Options;

namespace Core.Shell.Common.Commands
{
	public interface ICommand
	{
		RedirectableTextWriter Output { get; }

		RedirectableTextWriter Error { get; }

		void Execute (string invokedExecutableName, string[] parameters, ExecutionEnvironment env);
	}
}
