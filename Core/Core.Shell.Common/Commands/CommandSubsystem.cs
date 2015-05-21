using System;

namespace Core.Shell.Common.Commands
{
	public static class CommandSubsystems
	{
		public static CommandSubsystem[] Subsystems = new CommandSubsystem[] {
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

	public abstract class CommandSubsystem
	{
		public abstract bool ContainsCommand (string commandName);

		public abstract ICommand GetCommand (string commandName);
	}

	public interface ICommand
	{
		Action<string> Output { get; set; }

		void Execute (string[] parameters, ExecutionEnvironment env);
	}

	public abstract class AbstractCommand : ICommand
	{
		public Action<string> Output { get; set; }

		public abstract string ExecutableName { get; }

		#region ICommand implementation

		protected string[] parameters;
		protected ExecutionState state;
		protected ExecutionEnvironment env;

		public void Execute (string[] parameters, ExecutionEnvironment env)
		{
			this.parameters = parameters;
			this.state = new ExecutionState ();
			this.env = env;

			Output = env.Output;

			ExecuteInternal ();

			StackTraceElement element = new StackTraceElement {
				Executable = ExecutableName,
				Parameters = parameters,
				State = new ExecutionState {
					ExitCode = 0
				}
			};
			env.StackTrace.Add (element);
		}

		#endregion

		protected abstract void ExecuteInternal ();
	}
}

