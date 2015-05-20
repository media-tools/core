using System;

namespace Core.Common
{
	public static class ExceptionExtensions
	{
		public static Action<T> ThrowAction<T> (this Exception ex)
		{
			return (a) => {
				throw ex;
			};
		}
	}
}

