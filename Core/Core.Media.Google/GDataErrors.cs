using System;
using Core.Common;
using Google.GData.Client;

namespace Core.Google.Auth
{
	/*
	public class GDataErrors
	{
		public void CatchErrors (Action todo)
		{
			string dummy;
			CatchErrors (todo: todo, errorMessage: out dummy, catchAllExceptions: false, retryTimes: 1, afterRefresh: null);
		}

		public void CatchErrors (Action todo, Action afterRefresh)
		{
			string dummy;
			CatchErrors (todo: todo, errorMessage: out dummy, catchAllExceptions: false, retryTimes: 1, afterRefresh: afterRefresh);
		}

		public void CatchErrors (Action todo, out string errorMessage, bool catchAllExceptions, int retryTimes)
		{
			CatchErrors (todo: todo, errorMessage: out errorMessage, catchAllExceptions: catchAllExceptions, retryTimes: retryTimes, afterRefresh: null);
		}

		public void CatchErrors (Action todo, out string errorMessage, bool catchAllExceptions, int retryTimes, Action afterRefresh)
		{
			int triesLeft = retryTimes;
			do {
				try {
					todo ();
					errorMessage = null;
				} catch (GDataRequestException ex) {
					if (ex.InnerException != null && ex.InnerException.Message.Contains ("wrong scope")) {
						Log.Error ("GDataRequestException: ", ex.ResponseString);
						account.Reauthenticate ();
						todo ();
					} else {
						Log.Error ("GDataRequestException: ", ex.ResponseString);
						Log.Error (ex);
						if (ex.ResponseString.Contains ("Token invalid")) {
							Log.Error ("Refresh Account!");
							RefreshAccount ();
							if (afterRefresh != null) {
								afterRefresh ();
							}
						}
						// Log.Error (ex);
					}
					errorMessage = ex.ResponseString;
				} catch (ClientFeedException ex) {
					Log.Error ("ClientFeedException: ", ex.InnerException);
					Log.Error (ex);
					errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
				} catch (Exception ex) {
					errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
					if (catchAllExceptions) {
						Log.Error (ex);
					} else {
						throw;
					}
				}
			} while (errorMessage != null && triesLeft-- > 0);
		}
	}
	*/
}
