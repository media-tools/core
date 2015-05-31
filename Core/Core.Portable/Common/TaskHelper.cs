using System;
using System.Threading.Tasks;

namespace Core.Common
{
	public static class TaskHelper
	{
		public static Task Completed { get { return Task.FromResult (0); } }
	}
}

