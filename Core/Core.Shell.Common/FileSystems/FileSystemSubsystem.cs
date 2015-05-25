using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common;

namespace Core.Shell.Common.FileSystems
{
	public static class FileSystemSubsystems
	{
		public static List<FileSystemSubsystem> Subsystems = new List<FileSystemSubsystem> {
		};

		public static IVirtualDirectory DefaultRootDirectory { get; set; }

		public static IVirtualFile File (string path)
		{
			return getElement (path: path, internalFunc: (s, p) => s.File (p));
		}

		public static IVirtualDirectory Directory (string path)
		{
			return getElement (path: path, internalFunc: (s, p) => s.Directory (p));
		}

		public static IVirtualLink Link (string path)
		{
			return getElement (path: path, internalFunc: (s, p) => s.Link (p));
		}

		public static IVirtualNode Node (string path)
		{
			return getElement (path: path, internalFunc: (s, p) => s.Node (p));
		}

		static T getElement<T> (string path, Func<FileSystemSubsystem,string,T> internalFunc) where T : class
		{
			T result = null;
			foreach (FileSystemSubsystem subsystem in Subsystems) {
				result = internalFunc (subsystem, path);
				if (result != null) {
					break;
				}
			}
			return result;
		}

		public static Func<string, string> ResolveRelativePath (ExecutionEnvironment env)
		{
			return possibleRelativePath => {
				// check if it's recognized by any filesystem prefix!
				foreach (FileSystemSubsystem subsystem in Subsystems) {
					foreach (string prefix in subsystem.Prefixes) {
						if (possibleRelativePath.StartsWith (prefix)) {
							return prefix + FileSystemHelper.FormatPathWithoutPrefix (path: possibleRelativePath.Substring (prefix.Length), preserveFrontSlash: false);
						}
					}
				}

				// seems to be a relative path!
				string absolutePath = FileSystemHelper.CombinePath (env.WorkingDirectory, possibleRelativePath);
				return absolutePath;
			};
		}

		public static IVirtualNode ParseNativePath (string nativePath)
		{
			nativePath = FileSystemHelper.FormatNativePath (nativePath: nativePath);

			IVirtualNode result = null;
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

		public IVirtualDirectory DefaultRootDirectory { get; protected set; }

		protected void AddPrefix (params string[] prefix)
		{
			Prefixes = Prefixes.Extend (prefix);
		}

		public IVirtualFile File (string path)
		{
			return getElement (path: path, internalFunc: File);
		}

		public IVirtualDirectory Directory (string path)
		{
			return getElement (path: path, internalFunc: Directory);
		}

		public IVirtualLink Link (string path)
		{
			return getElement (path: path, internalFunc: Link);
		}

		public IVirtualNode Node (string path)
		{
			return getElement (path: path, internalFunc: Node);
		}

		T getElement<T> (string path, Func<string,string,T> internalFunc) where T : class
		{
			foreach (string prefix in Prefixes) {
				if (path.StartsWith (prefix)) {
					try {
						return internalFunc (prefix, path.Substring (prefix.Length));
					} catch (Exception ex) {
						throw new VirtualIOException (message: ex.Message, path: path, innerException: ex);
					}
				}
			}
			return null;
		}

		protected abstract IVirtualFile File (string prefix, string path);

		protected abstract IVirtualDirectory Directory (string prefix, string path);

		protected abstract IVirtualLink Link (string prefix, string path);

		protected abstract IVirtualNode Node (string prefix, string path);

		public abstract IVirtualNode ParseNativePath (string nativePath);
	}

}

