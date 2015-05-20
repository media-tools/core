using System;
using Core.Common;

namespace Core.Shell.Common
{
	public class Executer
	{
		public Executer ()
		{
		}

		public void Execute (ScriptBlock block)
		{
			ExecutionState state = new ExecutionState ();
			ExecuteBlock (block, ref state);
		}

		void ExecuteBlock (Block block, ref ExecutionState state)
		{
			if (state.IsAborted)
				return;
			
			var simpleBlock = block as ISimpleBlock;
			var conditionalBlock = block as IConditionalBlock;
			var commandBlock = block as ICommandBlock;

			if (simpleBlock != null) {
				ExecuteSimpleBlock (simpleBlock, ref state);
			} else if (conditionalBlock != null) {
				ExecuteConditionalBlock (conditionalBlock, ref state);
			} else if (commandBlock != null) {
				ExecuteCommandBlock (commandBlock, ref state);
			} else {
				Log.Error ("Unknown block: ", block);
			}
		}

		void ExecuteSimpleBlock (ISimpleBlock block, ref ExecutionState state)
		{
			if (state.IsAborted)
				return;
			
			foreach (Block subBlock in block.Content) {
				if (state.IsAborted)
					break;
				ExecuteBlock (block: subBlock, state: ref state);
			}
		}

		void ExecuteConditionalBlock (IConditionalBlock block, ref ExecutionState state)
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
				state.IsFatalError = true;
				return;
			}

			foreach (ICommandBlock command in block.Condition) {
				ExecuteCommandBlock (block: command, state: ref state);
			}

			if (state.IsExitSuccess) {
				Log.Debug ("Condition: Success!");



			} else {
				Log.Debug ("Condition: Failure!");

			}
		}

		void ExecuteCommandBlock (ICommandBlock block, ref ExecutionState state)
		{
			if (!string.IsNullOrWhiteSpace (block.ContentString)) {
				Log.Debug ("Execute command: ", block.ContentString);
				Command command = new Command (block);
				command.Execute (state: ref state);
			}
		}
	}
}

