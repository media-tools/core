using System;

namespace Core.Shell.Common.FileSystems
{
	public abstract class VirtualFile : VirtualNode
	{
		protected VirtualFile ()
		{
		}

		public abstract VirtualFileReader OpenReader ();

		public abstract VirtualFileWriter OpenWriter ();
	}
}

