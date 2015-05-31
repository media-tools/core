using System;
using System.Collections.Generic;
using Core.Common;
using Core.IO.Streams;
using Core.Portable;
using Core.Shell.Common.FileSystems;

namespace Core.Shell.Common
{
	public class ExecutionEnvironment
	{
		public FlexibleStream Output { get; } = new FlexibleStream ();

		public FlexibleStream Error { get; } = new FlexibleStream ();

		public FlexibleStream Input { get; } = new FlexibleStream ();

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

		public VirtualDirectory HomeDirectory { get; set; }

		public bool IsFatalError { protected get; set; } = false;

		public bool IsAborted { get { return IsFatalError; } }

		public ExecutionEnvironment ()
		{
			WorkingDirectory = findWorkingDirectory ();
			HomeDirectory = findHomeDirectory ();
		}

		VirtualDirectory findWorkingDirectory ()
		{
			VirtualDirectory dir;

			dir = FileSystemSubsystems.ParseNativePath (PlatformInfo.System.WorkingDirectory) as VirtualDirectory;
			if (dir != null) {
				return dir;
			}

			dir = FileSystemSubsystems.ParseNativePath (PlatformInfo.User.HomeDirectory) as VirtualDirectory;
			if (dir != null) {
				return dir;
			}

			dir = FileSystemSubsystems.DefaultRootDirectory;

			return dir;
		}

		VirtualDirectory findHomeDirectory ()
		{
			VirtualDirectory dir;

			dir = FileSystemSubsystems.ParseNativePath (PlatformInfo.User.HomeDirectory) as VirtualDirectory;
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

