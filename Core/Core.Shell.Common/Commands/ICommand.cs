using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Common;
using Core.Shell.Common.Streams;
using Mono.Options;

namespace Core.Shell.Common.Commands
{
	public interface ICommand
	{
		FlexibleStream Output { get; }

		FlexibleStream Error { get; }

		Task ExecuteAsync (string invokedExecutableName, string[] parameters, ExecutionEnvironment env);
	}
}
