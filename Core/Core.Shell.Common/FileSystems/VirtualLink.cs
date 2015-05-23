using System;

namespace Core.Shell.Common.FileSystems
{
	public abstract class VirtualLink : VirtualNode
	{
		protected VirtualLink (string prefix, string path)
			: base (prefix: prefix, path: path)
		{
		}
	}
}
