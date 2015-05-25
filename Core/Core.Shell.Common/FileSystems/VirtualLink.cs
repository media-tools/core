using System;

namespace Core.Shell.Common.FileSystems
{
	public abstract class VirtualLink : VirtualNode
	{
		protected VirtualLink (Path path)
			: base (path: path)
		{
		}
	}
}
