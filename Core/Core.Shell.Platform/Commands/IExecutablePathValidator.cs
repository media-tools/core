using System;
using System.Collections.Generic;
using System.IO;
using Core.Common;
using Core.IO;
using Core.Shell.Common.Commands;

namespace Core.Shell.Platform.Commands
{
	public interface IExecutablePathValidator
	{
		bool IsValidExecutable (string fullPath);
	}


}

