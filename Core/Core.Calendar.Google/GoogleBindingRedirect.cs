using System;
using Core.Common;

namespace Core.Calendar.Google
{
	public static class GoogleBindingRedirect
	{
		public static void Apply ()
		{
			BindingRedirect.RedirectAssembly (shortName: "System.Net.Http.Primitives", targetVersion: new Version (2, 2, 28, 0), publicKeyToken: "b03f5f7f11d50a3a");
		}
	}
}

