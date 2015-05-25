using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common;
using Core.IO;

namespace Core.Shell.Common.FileSystems
{
	public static class FileSystemSubsystems
	{
		public static List<FileSystemSubsystem> Subsystems = new List<FileSystemSubsystem> {
		};

		public static VirtualDirectory DefaultRootDirectory { get; set; }

		public static VirtualFile File (Path path)
		{
			return getElement (path: path, internalFunc: (s, p) => s.File (p));
		}

		public static VirtualDirectory Directory (Path path)
		{
			return getElement (path: path, internalFunc: (s, p) => s.Directory (p));
		}

		public static VirtualLink Link (Path path)
		{
			return getElement (path: path, internalFunc: (s, p) => s.Link (p));
		}

		public static VirtualNode Node (Path path)
		{
			return getElement (path: path, internalFunc: (s, p) => s.Node (p));
		}

		static T getElement<T> (Path path, Func<FileSystemSubsystem,Path,T> internalFunc) where T : class
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

		public static Func<string, Path> ResolveRelativePath (ExecutionEnvironment env)
		{
			return possibleRelativePath => {
				// check if it's recognized by any filesystem prefix!
				foreach (FileSystemSubsystem subsystem in Subsystems) {
					foreach (Prefix prefix in subsystem.Prefixes) {
						if (prefix.Matches (possibleRelativePath)) {
							return prefix.CreatePath (possibleRelativePath);
						}
					}
				}

				// seems to be a relative path!
				Path absolutePath = env.WorkingDirectory.Path.CombinePath (possibleRelativePath);
				return absolutePath;
			};
		}

		public static VirtualNode ParseNativePath (string nativePath)
		{
			nativePath = PathHelper.NormalizePath (path: nativePath);

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
		public Prefix[] Prefixes { get; protected set; } = new Prefix[0];

		public VirtualDirectory DefaultRootDirectory { get; protected set; }

		protected void AddPrefix (params Prefix[] prefix)
		{
			Prefixes = Prefixes.Extend (prefix);
		}

		public VirtualFile File (Path path)
		{
			return getElement (path: path, internalFunc: File);
		}

		public VirtualDirectory Directory (Path path)
		{
			return getElement (path: path, internalFunc: Directory);
		}

		public VirtualLink Link (Path path)
		{
			return getElement (path: path, internalFunc: Link);
		}

		public VirtualNode Node (Path path)
		{
			return getElement (path: path, internalFunc: Node);
		}

		T getElement<T> (Path path, Func<Path,T> internalFunc) where T : class
		{
			foreach (Prefix prefix in Prefixes) {
				if (prefix == path.Prefix) {
					try {
						return internalFunc (path);
					} catch (Exception ex) {
						throw new VirtualIOException (message: ex.Message, node: path, innerException: ex);
					}
				}
			}
			return null;
		}

		protected abstract VirtualFile FileInternal (Path path);

		protected abstract VirtualDirectory DirectoryInternal (Path path);

		protected abstract VirtualLink LinkInternal (Path path);

		protected abstract VirtualNode NodeInternal (Path path);

		public abstract VirtualNode ParseNativePath (string nativePath);

		public abstract string GetRealPath (Prefix prefix, string[] virtualPath);
	}

}

