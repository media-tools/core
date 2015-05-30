using System;
using Core.Shell.Common.Commands;
using System.Collections.Generic;
using Core.Common;
using Core.IO;

namespace Core.Shell.Platform.Commands
{
	public sealed class NativeExecutable
	{
		internal static readonly string[] IgnoreExtensions = new string[] { ".exe" };

		public string FullPath { get; private set; }

		public string FileName { get; private set; }

		public NativeExecutable (string fullPath)
		{
			FullPath = fullPath;
			FileName = PathHelper.GetFileName (fullPath);
		}

		public IEnumerable<string> CommandNames {
			get {
				yield return FullPath;
				yield return FileName;
				foreach (string ignoreExtension in IgnoreExtensions) {
					if (FileName.EndsWith (ignoreExtension, StringComparison.OrdinalIgnoreCase)) {
						int ignoreExtensionLength = ignoreExtension.Length;
						yield return FullPath.Substring (0, FullPath.Length - ignoreExtensionLength);
						yield return FileName.Substring (0, FileName.Length - ignoreExtensionLength);
					}
				}
			}
		}

		public override string ToString ()
		{
			return string.Format ("[NativeExecutable: FullPath={0}]", FullPath);
		}
	}
}

