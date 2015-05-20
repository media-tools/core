using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common;
using Newtonsoft.Json;

namespace Core.Shell.Common
{
	public abstract class Block
	{
		[JsonProperty ("block_type")]
		public BlockType Type { get; protected set; }

		[JsonIgnore]
		public string[] Begin { get; protected set; }

		[JsonIgnore]
		public string[] End { get; protected set; }

		[JsonIgnore]
		public BlockType[] AllowedContent { get; protected set; }

		[JsonIgnore]
		public bool EatEnd { get; protected set; } = true;

		[JsonIgnore]
		public List<Func<Block, bool>> ConstraintsContent { get; } = new List<Func<Block, bool>>();

		[JsonIgnore]
		public List<Func<Block, BlockType, bool>> ConstraintsAddContent { get; } = new List<Func<Block, BlockType, bool>>();

		[JsonProperty ("content")]
		public List<Block> Content { get; } = new List<Block> ();

		[JsonIgnore]
		public bool IsValid { get; protected set; } = true;

		public bool StartsWith (ref string script)
		{
			// check for beginning
			foreach (string begin in Begin) {
				if (script.StartsWith (begin)) {
					return true;
				}
			}
			if (Begin.Length == 0) {
				return true;
			}
			return false;
		}

		public bool EndsWith (ref string script)
		{
			foreach (string end in End) {
				//Log.Debug ("EndsWith: result: ", script.EndsWith (end), ", end: ", end, ", script: ", script);
				if (script.StartsWith (end)) {
					if (EatEnd) {
						script = script.Substring (end.Length);
					}
					return true;
				}
			}
			return false;
		}

		public virtual void Eat (ref string script)
		{
			// eat beginning
			foreach (string begin in Begin) {
				if (script.StartsWith (begin)) {
					script = script.Substring (begin.Length);
					break;
				}
			}

			// eat content
			bool full = false;
			while (script.Length > 0) {
				int previousLength = script.Length;

				if (EndsWith (script: ref script)) {
					break;
				}

				if (ConstraintsContent.Any (constraint => constraint (this))) {
					// content is full!
					full = true;
					break;
				}

				foreach (BlockType blockType in AllowedContent) {
					Block block = Blocks.CreateBlock (blockType);
					if (block.StartsWith (script: ref script)) {

						if (ConstraintsAddContent.Any (constraint => constraint (this, blockType))) {
							// content is full!
							full = true;
							break;
						}

						block.Eat (script: ref script);
						if (block.IsValid) {
							Content.Add (block);
						}
						break;
					}
				}

				if (!full && previousLength == script.Length) {
					Log.Warning ("Stuck at: ");
					Log.Indent++;
					Log.Warning (script);
					Log.Indent--;
					break;
				}
			}

			// trim script start
			Blocks.TrimStart (script: ref script);
		}

		/*IEnumerable<Block> parseBlocks (string script)
		{
			List<char> currentLine = new List<char> ();
			Dictionary<char,bool> inside = new Dictionary<char, bool> ();
			for (int i = 0; i < script.Length; i++) {

			}
		}*/

		public virtual bool ShouldSerializeContent ()
		{
			return true;
		}
	}

	public static class ContentConstraint
	{
		public static Func<Block, bool> OneOfEachType ()
		{
			return block => {
				if (block.AllowedContent.All (ac => block.Content.Any (c => c.Type == ac))) {
					Log.Debug ("ContentConstraint.OneOfEachType");
					return true;
				} else {
					return false;
				}
			};
		}

		public static Func<Block, BlockType, bool> OneOfType (params BlockType[] types)
		{
			return (block, newBlockType) => {
				if (types.All (ac => block.Content.Any (c => c.Type == ac)) && types.Contains (newBlockType)) {
					Log.Debug ("ContentConstraint.OneOfType: ", string.Join ("", types));
					return true;
				} else {
					return false;
				}
			};
		}
	}

	public class ScriptBlock : Block, ISimpleBlock
	{
		public ScriptBlock ()
		{
			Type = BlockType.SCRIPT;
			Begin = new string[] { };
			End = new string[] { };
			AllowedContent = Blocks.AllowedContent_ScriptBlock;
		}
	}

	public class CommandBlock : Block, ICommandBlock
	{
		[JsonProperty ("content_string")]
		public string ContentString { get; private set; }

		public CommandBlock ()
		{
			Type = BlockType.COMMAND;
			Begin = new string[] { };
			End = new string[] { "\n", "\r\n", ";" };
			AllowedContent = new BlockType[] {
				
			};
		}

		public override void Eat (ref string script)
		{
			IsValid = false;

			// eat content
			while (script.Length > 0 && !EndsWith (script: ref script)) {
				ContentString += script [0];
				script = script.Substring (1);

				IsValid = true;
			}

			// trim script start
			Blocks.TrimStart (script: ref script);
		}

		public override bool ShouldSerializeContent ()
		{
			return false;
		}
	}

	public abstract class AbstractConditionalBlock : Block, IConditionalBlock
	{
		[JsonIgnore]
		public ICommandBlock[] Condition { get { return Content.Where (b => b.Type == BlockType.COMMAND).Cast<ICommandBlock> ().ToArray (); } }

		[JsonIgnore]
		public Block[] ThenBlock { get { return Content.Where (b => b.Type == BlockType.THEN).ToArray (); } }

		[JsonIgnore]
		public Block[] ElseBlock { get { return Content.Where (b => b.Type == BlockType.ELIF || b.Type == BlockType.ELSE).ToArray (); } }
	}

	public class IfBlock : AbstractConditionalBlock
	{
		public IfBlock ()
		{
			Type = BlockType.IF;
			Begin = new string[] { "if " };
			End = new string[] { "fi" };
			AllowedContent = new BlockType[] {
				BlockType.THEN,
				BlockType.ELIF,
				BlockType.ELSE,
				BlockType.COMMAND,
			};

			//ConstraintsAddContent.Add (ContentConstraint.OneOfType (BlockType.COMMAND));
		}
	}

	public class ElifBlock : AbstractConditionalBlock
	{
		public ElifBlock ()
		{
			Type = BlockType.ELIF;
			Begin = new string[] { "elif " };
			End = new string[] { };
			AllowedContent = new BlockType[] {
				BlockType.THEN,
				BlockType.COMMAND,
			};
			ConstraintsContent.Add (ContentConstraint.OneOfEachType ());
		}
	}

	public class ElseBlock : Block, ISimpleBlock
	{
		public ElseBlock ()
		{
			Type = BlockType.ELSE;
			Begin = new string[] { "else " };
			End = new string[] { "fi" };
			EatEnd = false;
			AllowedContent = Blocks.AllowedContent_ScriptBlock;
		}
	}

	public class ThenBlock : Block, ISimpleBlock
	{
		public ThenBlock ()
		{
			Type = BlockType.THEN;
			Begin = new string[] { "then" };
			End = new string[] { "fi", "elif", "else" };
			EatEnd = false;
			AllowedContent = Blocks.AllowedContent_ScriptBlock;
		}
	}
}
