using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Common;
using Core.IO.Streams;

namespace Core.IO.Terminal
{
	public class ResponsiveReadLine : IHistoryReadLine, IDisposable
	{
		public CancellationToken CancelToken { get; set; } = CancellationToken.None;

		public bool IsOpen { get { return console.IsOpen; } }

		public string Line { get; private set; }

		public SpecialCommands SpecialCommand { get; private set; }

		public InputHistory History { get; set; }

		readonly object _lock = new object ();
		readonly ITerminalStream console;
		readonly IDisposable readEventChanger;
		TaskCompletionSource<bool> tcs;
		StringBuilder buf;

		public ResponsiveReadLine (ITerminalStream console)
		{
			this.console = console;
			readEventChanger = console.HandleReadEvent (readKey);
		}

		#region IDisposable implementation

		void IDisposable.Dispose ()
		{
			readEventChanger.Dispose ();
		}

		#endregion

		public Task<bool> TryReadLineAsync ()
		{
			lock (_lock) {
				// reset state
				tcs = new TaskCompletionSource<bool> ();
				buf = new StringBuilder ();
				Line = null;
				SpecialCommand = SpecialCommands.None;

				CancelToken.Register (() => finishRead (false));

				return tcs.Task;
			}
		}

		void finishRead (bool? result = null)
		{
			lock (_lock) {
				if (readEventChanger != null && buf != null && tcs != null) {
					Line = buf.ToString ();
					tcs.SetResult (result.HasValue ? result.Value : !string.IsNullOrEmpty (Line));

					buf = null;
					tcs = null;
				}
			}
		}

		async Task readKey (PortableConsoleKeyInfo key)
		{
			lock (_lock) {
				if (CancelToken.IsCancellationRequested) {
					finishRead ();
					return;
				}
				if (buf == null || tcs == null || readEventChanger == null) {
					Log.Warning ("Bug: ResponsiveReadLine: variable is null: ", $"buf={buf},tcs={tcs},readEventChanger={readEventChanger}");
					return;
				}
			}

			//if (ConsoleInput.TryReadKey (result: out key, cancelToken: CancelToken)) {
			//await console.WriteLineAsync (key.Key + " " + key.Modifiers);
			//Log.Debug ("Index: ", history.Index, ", History: ", history.History.Select (l => "\"" + l + "\"").Join (", "));

			// Ctrl-D
			if (key.Key == ConsoleKey.D && key.Modifiers == ConsoleModifiers.Control) {
				SpecialCommand = SpecialCommands.CloseStream;
				finishRead (true);
				return;
			}
			// F4 ? WTF ?
			else if (key.Key == ConsoleKey.F4) {
				SpecialCommand = SpecialCommands.CloseStream;
				finishRead (true);
				return;
			}
			// Arrow Keys
			else if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.LeftArrow || key.Key == ConsoleKey.RightArrow) {

				// save at current history position
				//history.Edit (buf.ToString ());

				// remove all current chars
				while (buf.Length > 0) {
					buf.Remove (buf.Length - 1, 1);
					await console.WriteAsync ("\b \b");
				}

				if (History != null) {
					// move inside the history
					if (key.Key == ConsoleKey.UpArrow)
						History.Up ();
					else if (key.Key == ConsoleKey.DownArrow)
						History.Down ();

					// load the current history position
					buf.Append (History.Current);
					await console.WriteAsync (buf);
				}
			}
			// Enter
			else if (key.Key == ConsoleKey.Enter) {
				if (History != null) {
					History.Add (buf.ToString ());
				}
				await console.WriteLineAsync ();
				finishRead (true);
				return;
			}
			// Backspace
			else if (key.Key == ConsoleKey.Backspace) {
				if (buf.Length > 0) {
					buf.Remove (buf.Length - 1, 1);
					await console.WriteAsync ("\b \b");
				}
			}
			// normal character
			else if (key.KeyChar != 0) {
				buf.Append (key.KeyChar);
				await console.WriteAsync (key.KeyChar);
			}
		}
	}
}

