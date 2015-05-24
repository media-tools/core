using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common;
using Mono.Options;

namespace Core.Shell.Common.Commands
{
	public abstract class AbstractCommand : ICommand
	{
		// the default output stream
		public RedirectableTextWriter Output { get; } = new RedirectableTextWriter();
		// the default error stream
		public RedirectableTextWriter Error { get; } = new RedirectableTextWriter();

		// the default executable name
		public string ExecutableName { get; protected set; } = "unknown";

		// Mono.Options
		protected OptionSet optionSet = new OptionSet ();
		protected bool UseOptions = false;
		protected bool printHelp = false;

		// the invoked executable name
		protected string invokedExecutableName;

		// Parameters
		protected List<string> parameters;

		// State
		protected ExecutionState state;

		// Environment
		protected ExecutionEnvironment env;

		protected AbstractCommand ()
		{
			optionSet.Add ("help|?", "Prints out the options.", option => printHelp = (option != null));
			// Analysis disable once ConvertClosureToMethodGroup
			optionSet.Add ("<>", p => parameters.Add (p));
		}

		#region ICommand implementation

		public void Execute (string invokedExecutableName, string[] parameters, ExecutionEnvironment env)
		{
			Log.Debug ("Execute: ", ExecutableName, " ", parameters.ToJson (inline: true));
			Output.Stream = env.Output.Stream;
			Error.Stream = env.Error.Stream;

			this.invokedExecutableName = invokedExecutableName;
			this.parameters = parameters.ToList ();
			this.state = new ExecutionState ();
			this.env = env;

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
			string[] unparsedParameters = parameters.ToArray ();
			parameters.Clear ();
			try {
				optionSet.Parse (unparsedParameters);
			} catch (OptionException) {
				printHelp = true;
			}
		}

		#endregion

		protected abstract void ExecuteInternal ();

		protected abstract void ResetInternalState ();
	}
}
