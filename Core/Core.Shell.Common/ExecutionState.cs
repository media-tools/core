using System;
using Core.Common;
using System.Collections.Generic;

namespace Core.Shell.Common
{
	public class ExecutionState
	{
		public static readonly int EXIT_SUCCESS = 0;

		public int ExitCode { get; set; } = 0;

		public List<string> StackTrace { get; set; } = new List<string>();

		public bool IsFatalError { protected get; set; } = false;

		public bool IsAborted { get { return IsFatalError; } }

		public bool IsExitSuccess { get { return ExitCode == EXIT_SUCCESS; } }

		public string WorkingDirectory { get; set; }
	}

}

