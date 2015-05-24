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

		public static VirtualDirectory DefaultRootDirectory { get; set; }

		public static VirtualFile File (string path)
		{
			return getElement (path: path, internalFunc: (s, p) => s.File (p));
		}

		public static VirtualDirectory Directory (string path)
		{
			return getElement (path: path, internalFunc: (s, p) => s.Directory (p));
		}

		public static VirtualLink Link (string path)
		{
			return getElement (path: path, internalFunc: (s, p) => s.Link (p));
		}

		public static VirtualNode Node (string path)
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

		public static VirtualNode ParseNativePath (string nativePath)
		{
			nativePath = FileSystemHelper.FormatNativePath (nativePath: nativePath);

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

		public VirtualLink Link (string path)
		{
			return getElement (path: path, internalFunc: Link);
		}

		public VirtualNode Node (string path)
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

		protected abstract VirtualFile File (string prefix, string path);

		protected abstract VirtualDirectory Directory (string prefix, string path);

		protected abstract VirtualLink Link (string prefix, string path);

		protected abstract VirtualNode Node (string prefix, string path);

		public abstract VirtualNode ParseNativePath (string nativePath);
	}

	public static class FileSystemHelper
	{
		public static string FormatNativePath (string nativePath)
		{
			// no windows file separators!
			nativePath = nativePath.Replace ('\\', '/').TrimEnd ('/');

			nativePath = FormatPathWithoutPrefix (path: nativePath, preserveFrontSlash: true);

			return nativePath;
		}

		public static string FormatPathWithoutPrefix (string path, bool preserveFrontSlash)
		{
			path = ResolvePath (path: path, preserveFrontSlash: preserveFrontSlash);

			if (path.ToCharArray ().Any (c => c != '/')) {
				path = path.TrimEnd ('/');
			}
			return path;
		}

		public static string CombinePath (params string[] parts)
		{
			if (parts.Length == 0) {
				return string.Empty;
			}

			// it doesn't really matter whether we set "preserveFrontSlash" to true or false
			return FormatPathWithoutPrefix (path: string.Join ("/", parts.Where (p => p.Length > 0)), preserveFrontSlash: true);
		}

		public static string CombinePath (VirtualDirectory first, params string[] parts)
		{
			return first.VirtualPrefix + CombinePath (first.VirtualPath.Extend (parts));
		}

		private static string ResolvePath (string path, bool preserveFrontSlash)
		{
			bool startsWithSlash = path.StartsWith ("/");

			string[] oldParts = path.Split (new []{ '/' }, StringSplitOptions.RemoveEmptyEntries);
			List<string> newParts = new List<string> ();
			foreach (string part in oldParts) {
				if (part == ".") {
					// ignore
				} else if (part == "..") {
					// go one up!
					if (newParts.Count > 0) {
						newParts.RemoveAt (newParts.Count - 1);
					}
				} else if (string.IsNullOrEmpty (part)) {
					// empty path part!
				} else {
					// normal path part
					newParts.Add (part);
				}
			}

			path = newParts.Join ("/");
			if (preserveFrontSlash && startsWithSlash) {
				path = "/" + path;
			}
			return path;
		}
	}
}

