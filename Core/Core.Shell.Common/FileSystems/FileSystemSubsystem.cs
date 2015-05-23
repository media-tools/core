using System;
using System.Collections.Generic;
using Core.Common;

namespace Core.Shell.Common.FileSystems
{
	public static class FileSystemSubsystems
	{
		public static List<FileSystemSubsystem> Subsystems = new List<FileSystemSubsystem> {
		};

		public static VirtualDirectory DefaultRootDirectory { get; set; }

		public static VirtualFile File (string path)
		{
			VirtualFile result = null;
			foreach (FileSystemSubsystem subsystem in Subsystems) {
				result = subsystem.File (path);
				if (result != null) {
					break;
				}
			}
			return result;
		}

		public static VirtualDirectory Directory (string path)
		{
			VirtualDirectory result = null;
			foreach (FileSystemSubsystem subsystem in Subsystems) {
				result = subsystem.Directory (path);
				if (result != null) {
					break;
				}
			}
			return result;
		}

		public static VirtualNode Node (string path)
		{
			VirtualNode result = null;
			foreach (FileSystemSubsystem subsystem in Subsystems) {
				result = subsystem.Node (path);
				if (result != null) {
					break;
				}
			}
			return result;
		}

		public static VirtualNode ParseNativePath (string nativePath)
		{
			VirtualNode result = null;
			foreach (FileSystemSubsystem subsystem in Subsystems) {
				result = subsystem.ParseNativePath (nativePath);
				if (result != null) {
					break;
				}
			}
			return result;
		}
	}

	public abstract class FileSystemSubsystem
	{
		public string[] Prefixes { get; protected set; } = new string[0];

		public VirtualDirectory DefaultRootDirectory { get; protected set; }

		protected void AddPrefix (params string[] prefix)
		{
			Prefixes = Prefixes.Extend (prefix);
		}

		public VirtualFile File (string path)
		{
			return getElement (path: path, internalFunc: File);
		}

		public VirtualDirectory Directory (string path)
		{
			return getElement (path: path, internalFunc: Directory);
		}

		public VirtualNode Node (string path)
		{
			return getElement (path: path, internalFunc: Node);
		}

		T getElement<T> (string path, Func<string,string,T> internalFunc) where T : class
		{
			path = path.TrimEnd ('/');
			foreach (string prefix in Prefixes) {
				if (path.StartsWith (prefix)) {
					return internalFunc (prefix, path.Substring (prefix.Length));
				}
			}
			return null;
		}

		protected abstract VirtualFile File (string prefix, string path);

		protected abstract VirtualDirectory Directory (string prefix, string path);

		protected abstract VirtualNode Node (string prefix, string path);

		public abstract VirtualNode ParseNativePath (string nativePath);
	}
}

