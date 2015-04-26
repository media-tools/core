using System;
using Core.Calendar.Google;
using Core.Common;
using System.Reflection;
using System.Globalization;

namespace Core.Tests.Calendar.Google
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			RedirectAssembly (shortName: "System.Net.Http.Primitives", targetVersion: new Version (2, 2, 28, 0), publicKeyToken: "b03f5f7f11d50a3a");
			GoogleBindingRedirect.Apply ();

			Logging.Enable ();

			GoogleCalendarService google = new GoogleCalendarService (new TestConfig ());
		}

		///<summary>Adds an AssemblyResolve handler to redirect all attempts to load a specific assembly name to the specified version.</summary>
		public static void RedirectAssembly (string shortName, Version targetVersion, string publicKeyToken)
		{
			ResolveEventHandler handler = null;

			handler = (sender, args) => {
				Console.WriteLine ("May redirect assembly load of " + args.Name);
				// Use latest strong name & version when trying to load SDK assemblies
				var requestedAssembly = new AssemblyName (args.Name);
				if (requestedAssembly.Name != shortName)
					return null;

				Console.WriteLine ("Redirecting assembly load of " + args.Name + ",\tloaded by " + (args.RequestingAssembly == null ? "(unknown)" : args.RequestingAssembly.FullName));

				requestedAssembly.Version = targetVersion;
				requestedAssembly.SetPublicKeyToken (new AssemblyName ("x, PublicKeyToken=" + publicKeyToken).GetPublicKeyToken ());
				requestedAssembly.CultureInfo = CultureInfo.InvariantCulture;

				AppDomain.CurrentDomain.AssemblyResolve -= handler;

				return Assembly.Load (requestedAssembly);
			};
			AppDomain.CurrentDomain.AssemblyResolve += handler;
		}

		public class TestConfig : IGoogleConfig
		{
			#region IGoogleConfig implementation

			public string GoogleUser {
				get {
					return "fuck";
				}
			}

			#endregion

		}
	}
}
