using System;
using Core.Common;
using System.Linq;

namespace Core.Shell.Common
{
	public class Executer
	{
		public ExecutionEnvironment Environment { get; private set; } = new ExecutionEnvironment ();

		public Executer ()
		{
		}

		public void Execute (ScriptBlock block)
		{
			ExecuteBlock (block);
		}

		void ExecuteBlock (Block block)
		{
			if (Environment.IsAborted)
				return;
			
			var simpleBlock = block as ISimpleBlock;
			var conditionalBlock = block as IConditionalBlock;
			var commandBlock = block as ICommandBlock;

			if (simpleBlock != null) {
				ExecuteSimpleBlock (simpleBlock);
			} else if (conditionalBlock != null) {
				ExecuteConditionalBlock (conditionalBlock);
			} else if (commandBlock != null) {
				ExecuteCommandBlock (commandBlock);
			} else {
				Log.Error ("Unknown block: ", block);
			}
		}

		void ExecuteSimpleBlock (ISimpleBlock block)
		{
			if (Environment.IsAborted)
				return;
			
			foreach (Block subBlock in block.Content) {
				if (Environment.IsAborted)
					break;
				ExecuteBlock (block: subBlock);
			}
		}

		void ExecuteConditionalBlock (IConditionalBlock block)
		{
			Log.Debug ("Condition block: ");
			Log.Indent++;
			Log.Debug (block.Condition.ToJson ());
			Log.Indent--;

			Log.Debug ("Then block: ");
			Log.Indent++;
			Log.Debug (block.ThenBlock.ToJson ());
			Log.Indent--;

			Log.Debug ("Else block: ");
			Log.Indent++;
			Log.Debug (block.ElseBlock.ToJson ());
			Log.Indent--;

			if (block.Condition.Length == 0) {
				Log.Error ("Condition is empty!");
				Environment.IsFatalError = true;
				return;
			}

			foreach (ICommandBlock command in block.Condition) {
				ExecuteCommandBlock (block: command);
			}

			if (true) {//env.StackTrace.Last ().State.IsExitSuccess) {
				Log.Debug ("Condition: Success!");



			} else {
				Log.Debug ("Condition: Failure!");

			}
		}

		void ExecuteCommandBlock (ICommandBlock block)
		{
			Log.Warning ("Execute command: ", block.ContentString);
			CommandExecutor commandExecutor = new CommandExecutor (block: block);
			commandExecutor.Execute (env: Environment);
			/*
			try {
				System.Diagnostics.Debug.WriteLine ("fuck");
				Log.Debug ("Execute command: ", block.ContentString, (!string.IsNullOrWhiteSpace (block.ContentString)));
				if (!string.IsNullOrWhiteSpace (block.ContentString)) {
					Log.Debug ("Execute command: ", block.ContentString);
					CommandExecutor commandExecutor = new CommandExecutor (block);
					commandExecutor.Execute (state: ref env);
					Log.Debug ("test");
				}
				Log.Debug ("test");
			} catch (Exception ex) {
				Log.Error (ex);
				env.IsFatalError = true;
			}*/
		}
	}
}

