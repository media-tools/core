using System;
using Core.Common;
using System.Collections.Generic;
using Core.Shell.Common.FileSystems;
using Core.Portable;

namespace Core.Shell.Common
{
	public class ExecutionEnvironment
	{
		public RedirectableTextWriter Output { get; } = new RedirectableTextWriter ();

		public RedirectableTextWriter Error { get; } = new RedirectableTextWriter ();

		public List<StackTraceElement> StackTrace { get; set; } = new List<StackTraceElement> {
			new StackTraceElement {
				Executable = "root",
				Parameters = new string[0],
				State = new ExecutionState {
					ExitCode = 0,
				},
			}
		};

		public VirtualDirectory WorkingDirectory { get; set; }

		public bool IsFatalError { protected get; set; } = false;

		public bool IsAborted { get { return IsFatalError; } }

		public ExecutionEnvironment ()
		{
			WorkingDirectory = findWorkingDirectory ();
		}

		VirtualDirectory findWorkingDirectory ()
		{
			VirtualDirectory dir;

			dir = FileSystemSubsystems.ParseNativePath (SystemInfo.WorkingDirectory) as VirtualDirectory;
			if (dir != null) {
				return dir;
			}

			dir = FileSystemSubsystems.ParseNativePath (SystemInfo.WorkingDirectory) as VirtualDirectory;
			if (dir != null) {
				return dir;
			}

			dir = FileSystemSubsystems.DefaultRootDirectory;
			return dir;
		}
	}

	public class ExecutionState
	{
		public static readonly int EXIT_SUCCESS = 0;

		public int ExitCode { get; set; } = EXIT_SUCCESS;

		public bool IsExitSuccess { get { return ExitCode == EXIT_SUCCESS; } }
	}

	public struct StackTraceElement
	{
		public string Executable;
		public string[] Parameters;
		public ExecutionState State;
	}
}

