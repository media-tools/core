using System;
using System.Threading.Tasks;
using Core.Common;

namespace Core.IO.Terminal
{
	public delegate Task ConsoleReadHandler (PortableConsoleKeyInfo key);

	public interface ITerminalInputStream
	{
		ConsoleReadHandler ReadHandler { get; set; }

		//bool TryRead (out PortableConsoleKey result, int timeout);

		bool IsOpen { get; }
	}

	public static class ConsoleInputExtensions
	{
		public static IDisposable HandleReadEvent (this ITerminalInputStream consoleInput, ConsoleReadHandler readHandler)
		{
			return new HandleReadEventChanger (consoleInput: consoleInput, readHandler: readHandler);
		}

		private class HandleReadEventChanger : IDisposable
		{
			readonly ITerminalInputStream consoleInput;
			readonly ConsoleReadHandler previousReadHandler;

			public HandleReadEventChanger (ITerminalInputStream consoleInput, ConsoleReadHandler readHandler)
			{
				this.consoleInput = consoleInput;
				this.previousReadHandler = consoleInput.ReadHandler;
				consoleInput.ReadHandler = readHandler;
			}

			#region IDisposable implementation

			public void Dispose ()
			{
				consoleInput.ReadHandler = previousReadHandler;
			}

			#endregion
		}
	}
}

