using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Common;
using Core.IO.Streams;
using Core.Portable;

namespace Core.IO.Terminal
{
	public class ResponsiveReadLine : IHistoryReadLine, IDisposable
	{
		public bool IsOpen { get { return console.IsOpen; } }

		public ReadLineHandler Callback { get; set; }

		public InputHistory History { get; set; }

		//readonly object _lock = new object ();
		readonly ITerminalStream console;
		readonly IDisposable readEventChanger;

		bool isDisposed = false;
		InputLine line;

		public ResponsiveReadLine (ITerminalStream console)
		{
			this.console = console;
			line = new InputLine ();
			readEventChanger = console.HandleReadEvent (readHandler: ReadKey);
		}

		#region IDisposable implementation

		void IDisposable.Dispose ()
		{
			if (!isDisposed) {
				if (readEventChanger != null) {
					readEventChanger.Dispose ();
				}
				isDisposed = true;
			}
		}

		#endregion

		async Task SendResult ()
		{
			InputLine tmp = line;
			line = new InputLine ();
			await Callback (tmp).ConfigureAwait (false);
		}

		async Task ReadKey (PortableConsoleKeyInfo key)
		{
			if (isDisposed) {
				Log.Warning ("Bug in ResponsiveReadLine.ReadKey: isDisposed = true! key=", key);
				return;
			}

			//if (ConsoleInput.TryReadKey (result: out key, cancelToken: CancelToken)) {
			//await console.WriteLineAsync (key.Key + " " + key.Modifiers);
			//Log.Debug ("Index: ", history.Index, ", History: ", history.History.Select (l => "\"" + l + "\"").Join (", "));

			// Ctrl-D
			if (key.Key == ConsoleKey.D && key.Modifiers == ConsoleModifiers.Control) {
				line.SpecialCommand = SpecialCommands.CloseStream;
				await SendResult ();
				return;
			}
			// F4 ? WTF ?
			else if (key.Key == ConsoleKey.F4) {
				line.SpecialCommand = SpecialCommands.CloseStream;
				await SendResult ();
				return;
			}
			// Arrow Keys
			else if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.LeftArrow || key.Key == ConsoleKey.RightArrow) {

				// remove all current chars
				while (line.buf.Length > 0) {
					line.buf.Remove (line.buf.Length - 1, 1);
					await console.WriteAsync ("\b \b");
				}

				if (History != null) {
					// move inside the history
					if (key.Key == ConsoleKey.UpArrow)
						History.Up ();
					else if (key.Key == ConsoleKey.DownArrow)
						History.Down ();

					// load the current history position
					line.buf.Append (History.Current);
					await console.WriteAsync (line.buf);
				}
			}
			// Enter
			else if (key.Key == ConsoleKey.Enter) {
				if (History != null) {
					History.Add (line.buf.ToString ());
				}
				await console.WriteLineAsync ();
				await SendResult ();
				return;
			}
			// Backspace
			else if (key.Key == ConsoleKey.Backspace) {
				if (line.buf.Length > 0) {
					line.buf.Remove (line.buf.Length - 1, 1);
					await console.WriteAsync ("\b \b");
				}
			}
			// normal character
			else if (key.KeyChar != 0) {
				line.buf.Append (key.KeyChar);
				await console.WriteAsync (key.KeyChar);
			}
		}

		public class InputLine : ILine
		{
			internal StringBuilder buf = new StringBuilder ();

			public string Line { get { return buf.ToString (); } }

			public SpecialCommands SpecialCommand { get; internal set; } = SpecialCommands.None;

			public override string ToString ()
			{
				return string.Format ("[InputLine: Line={0}, SpecialCommand={1}]", Line, SpecialCommand);
			}
		}
	}
}

