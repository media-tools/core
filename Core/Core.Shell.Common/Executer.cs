﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Common;

namespace Core.Shell.Common
{
	public class Executer
	{
		public ExecutionEnvironment Environment { get; private set; } = new ExecutionEnvironment ();

		public Executer ()
		{
		}

		public async Task Execute (ScriptBlock block)
		{
			await ExecuteBlock (block);
		}

		async Task ExecuteBlock (Block block)
		{
			if (Environment.IsAborted)
				return;
			
			var simpleBlock = block as ISimpleBlock;
			var conditionalBlock = block as IConditionalBlock;
			var commandBlock = block as ICommandBlock;

			if (simpleBlock != null) {
				await ExecuteSimpleBlock (simpleBlock);
			} else if (conditionalBlock != null) {
				await ExecuteConditionalBlock (conditionalBlock);
			} else if (commandBlock != null) {
				await ExecuteCommandBlock (commandBlock);
			} else {
				Log.Error ("Unknown block: ", block);
			}
		}

		async Task ExecuteSimpleBlock (ISimpleBlock block)
		{
			if (Environment.IsAborted)
				return;
			
			foreach (Block subBlock in block.Content) {
				if (Environment.IsAborted)
					break;
				await ExecuteBlock (block: subBlock);
			}
		}

		async Task<bool> ExecuteConditionalBlock (IConditionalBlock block)
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
				return false;
			}

			foreach (ICommandBlock command in block.Condition) {
				await ExecuteCommandBlock (block: command);
			}

			bool wasConditionSuccessful = Environment.StackTrace.Last ().State.IsExitSuccess;

			if (wasConditionSuccessful) {
				Log.Debug ("Condition: Success!");

				Log.Indent++;
				foreach (Block thenBlock in block.ThenBlock) {
					await ExecuteBlock (block: thenBlock);
				}
				Log.Indent--;

			} else {
				Log.Debug ("Condition: Failure!");

				Log.Indent++;
				foreach (Block elifOrElseBlock in block.ElseBlock) {
					// if it's an ELIF block
					if (elifOrElseBlock.Type == BlockType.ELIF) {
						bool wasElifSuccessful = await ExecuteConditionalBlock (elifOrElseBlock as ElifBlock);
						if (wasElifSuccessful) {
							break;
						}
					}
					// if it's an ELSE block
					else if (elifOrElseBlock.Type == BlockType.ELSE) {
						await ExecuteBlock (block: elifOrElseBlock);
					}
					// WTF?
					else {
						throw new ArgumentException ("This should never happen! ElseBlock is neither ELIF nor ELSE!");
					}
				}
				Log.Indent--;
			}

			return wasConditionSuccessful;
		}

		async Task ExecuteCommandBlock (ICommandBlock block)
		{
			try {
				if (!string.IsNullOrWhiteSpace (block.ContentString)) {
					Log.Debug ("Execute: \"", block.ContentString, "\"");
					CommandExecutor commandExecutor = new CommandExecutor (block);
					await commandExecutor.ExecuteAsync (env: Environment);
				}
			} catch (Exception ex) {
				Log.Error (ex);
				Environment.IsFatalError = true;
			}
		}
	}
}

