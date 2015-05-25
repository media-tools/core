using System;
using System.IO;

namespace Core.Shell.Common.FileSystems
{
	public interface IVirtualNode
	{
		string VirtualPrefix { get; }

		string VirtualPath { get; }

		string VirtualFileName { get; }

		string PermissionsString { get; }

		string OwnerName { get; }

		string GroupName { get; }

		bool Validate (bool throwExceptions);
	}
}
