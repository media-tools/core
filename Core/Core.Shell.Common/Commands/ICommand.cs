using System.Threading.Tasks;
using Core.IO.Streams;

namespace Core.Shell.Common.Commands
{
	public interface ICommand
	{
		FlexibleStream Output { get; }

		FlexibleStream Error { get; }

		Task ExecuteAsync (string invokedExecutableName, string[] parameters, ExecutionEnvironment env);
	}
}
