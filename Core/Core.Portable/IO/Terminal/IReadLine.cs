using System.Threading;
using System.Threading.Tasks;

namespace Core.IO.Terminal
{
	public interface IReadLine
	{
		CancellationToken CancelToken { get; set; }

		string Line { get; }

		SpecialCommands SpecialCommand { get; }

		bool IsOpen { get; }

		Task<bool> TryReadLineAsync ();
	}

	public interface IHistoryReadLine : IReadLine
	{
		InputHistory History { get; set; }
	}

	public enum SpecialCommands
	{
		None = 0,
		CloseStream = 1,
		/*ArrowUp,
		ArrowDown,
		ArrowLeft,
		ArrowRight,*/
	}

}

