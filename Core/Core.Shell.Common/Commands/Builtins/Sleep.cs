using System;
using System.Linq;
using Core.Portable;

namespace Core.Shell.Common.Commands.Builtins
{
	public class Sleep : BuiltinCommand
	{
		public Sleep ()
		{
			ExecutableName = "sleep";
		}

		protected override void ResetInternalState ()
		{
		}

		protected override void ExecuteInternal ()
		{
			try {
				if (parameters.Count != 1) {
					throw new ArgumentException ("You need to provide an interval as the first command line parameter.");
				}

				string intervalStr = parameters [0];
				int multiplier;
				if (intervalStr.EndsWith ("ms"))
					multiplier = 1;
				else if (intervalStr.EndsWith ("s"))
					multiplier = 1000;
				else if (intervalStr.EndsWith ("m"))
					multiplier = 1000 * 60;
				else if (intervalStr.EndsWith ("h"))
					multiplier = 1000 * 60 * 60;
				else if (intervalStr.EndsWith ("d"))
					multiplier = 1000 * 60 * 60 * 24;
				else
					multiplier = 1000;
				intervalStr = new string (intervalStr.ToCharArray ().Where (c => char.IsDigit (c)).ToArray ());
				double interval = int.Parse (s: intervalStr);

				Thread.Sleep ((int)(interval * multiplier));

				state.ExitCode = 0;

			} catch (Exception ex) {
				Error.WriteLine (ex);
				state.ExitCode = 1;
			}
		}
	}
}

