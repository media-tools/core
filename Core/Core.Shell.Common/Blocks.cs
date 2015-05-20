using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Shell.Common
{
	public static class Blocks
	{
		public static Block[] AllBlocks {
			get {
				return Enum.GetValues (typeof(BlockType))
					.Cast<BlockType> ()
					.Where (bt => bt != BlockType.SCRIPT)
					.Select (bt => CreateBlock (bt)).ToArray ();
			}
		}

		public static Block CreateBlock (BlockType type)
		{
			switch (type) {
			case BlockType.COMMAND:
				return new CommandBlock ();
			case BlockType.IF:
				return new IfBlock ();
			case BlockType.ELIF:
				return new ElifBlock ();
			case BlockType.ELSE:
				return new ElseBlock ();
			case BlockType.THEN:
				return new ThenBlock ();
			default:
				return null;
			}
		}

		public static BlockType[] AllowedContent_ScriptBlock = new BlockType[] {
			BlockType.IF,
			BlockType.COMMAND,
		};

		public static void TrimStart (ref string script)
		{
			script = script.TrimStart ('\n', '\r', ' ', '\t');
		}
	}

	public enum BlockType
	{
		SCRIPT,
		COMMAND,
		IF,
		ELIF,
		ELSE,
		THEN,
	}

	public interface ISimpleBlock
	{
		List<Block> Content { get; }
	}

	public interface IConditionalBlock
	{
		ICommandBlock[] Condition { get; }

		Block[] ThenBlock { get; }

		Block[] ElseBlock { get; }
	}

	public interface ICommandBlock
	{
		string ContentString { get; }
	}
}
