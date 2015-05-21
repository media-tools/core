using System;
using Mono.Options;

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
		RedirectableTextWriter Output { get; }

		void Execute (string[] parameters, ExecutionEnvironment env);
	}

	public abstract class AbstractCommand : ICommand
	{
		// the default output stream
		public RedirectableTextWriter Output { get; } = new RedirectableTextWriter();

		// the default executable name
		public string ExecutableName { get; protected set; } = "unknown";

		// Mono.Options
		protected OptionSet optionSet = new OptionSet ();
		protected bool UseOptions = false;
		protected bool printHelp = false;

		// Parameters
		protected string[] parameters;

		// State
		protected ExecutionState state;

		// Environment
		protected ExecutionEnvironment env;

		protected AbstractCommand ()
		{
			optionSet.Add ("h|help|?", "Prints out the options.", option => printHelp = (option != null));
		}

		#region ICommand implementation

		public void Execute (string[] parameters, ExecutionEnvironment env)
		{
			this.parameters = parameters;
			this.state = new ExecutionState ();
			this.env = env;

			Output.Stream = env.Output.Stream;

			ResetInternalState ();
			if (UseOptions) {
				parseOptions ();
			}


			// need help ?
			if (printHelp) {
				printOptions ();
				return;
			}
			// execute the command
			else {
				ExecuteInternal ();
			}

			StackTraceElement element = new StackTraceElement {
				Executable = ExecutableName,
				Parameters = parameters,
				State = new ExecutionState {
					ExitCode = 0
				}
			};
			env.StackTrace.Add (element);
		}

		private void printOptions ()
		{
			optionSet.WriteOptionDescriptions (env.Output);
		}

		private void parseOptions ()
		{
			try {
				optionSet.Parse (parameters);
			} catch (OptionException) {
				printHelp = true;
			}
		}

		#endregion

		protected abstract void ExecuteInternal ();

		protected virtual void ResetInternalState ()
		{
		}
	}
}

