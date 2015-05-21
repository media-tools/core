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
			ExecutionEnvironment _state = Environment;
			ExecuteBlock (block, ref _state);
			Environment = _state;
		}

		void ExecuteBlock (Block block, ref ExecutionEnvironment env)
		{
			if (env.IsAborted)
				return;
			
			var simpleBlock = block as ISimpleBlock;
			var conditionalBlock = block as IConditionalBlock;
			var commandBlock = block as ICommandBlock;

			if (simpleBlock != null) {
				ExecuteSimpleBlock (simpleBlock, ref env);
			} else if (conditionalBlock != null) {
				ExecuteConditionalBlock (conditionalBlock, ref env);
			} else if (commandBlock != null) {
				ExecuteCommandBlock (commandBlock, ref env);
			} else {
				Log.Error ("Unknown block: ", block);
			}
		}

		void ExecuteSimpleBlock (ISimpleBlock block, ref ExecutionEnvironment env)
		{
			if (env.IsAborted)
				return;
			
			foreach (Block subBlock in block.Content) {
				if (env.IsAborted)
					break;
				ExecuteBlock (block: subBlock, env: ref env);
			}
		}

		void ExecuteConditionalBlock (IConditionalBlock block, ref ExecutionEnvironment env)
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
				env.IsFatalError = true;
				return;
			}

			foreach (ICommandBlock command in block.Condition) {
				ExecuteCommandBlock (block: command, env: ref env);
			}

			if (true) {//env.StackTrace.Last ().State.IsExitSuccess) {
				Log.Debug ("Condition: Success!");



			} else {
				Log.Debug ("Condition: Failure!");

			}
		}

		void ExecuteCommandBlock (ICommandBlock block, ref ExecutionEnvironment env)
		{
			Log.Warning ("Execute command: ", block.ContentString);
			CommandExecutor commandExecutor = new CommandExecutor (block);
			commandExecutor.Execute (state: ref env);
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

