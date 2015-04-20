using System;

namespace Core.Common
{
	public static class ExceptionHelper
	{
		public static void Catch<T> (Action tryAction, Action<Exception> catchAction) where T : Exception
		{
			try {
				tryAction ();
			} catch (T ex) {
				if (catchAction != null) {
					catchAction (ex);
				}
			} catch (System.Reflection.TargetInvocationException _ex) {
				Exception ex;
				if (_ex.InnerException != null)
					ex = _ex.InnerException;
				else
					ex = _ex;
				if (catchAction != null) {
					catchAction (ex);
				}
			}
		}
	}
}

