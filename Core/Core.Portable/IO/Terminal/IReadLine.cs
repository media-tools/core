using System.Threading;
using System.Threading.Tasks;
using Core.Common;

namespace Core.IO.Terminal
{
	public delegate Task ReadLineHandler (ILine line);

	public interface IReadLine
	{
		ReadLineHandler Callback { get; set; }

		bool IsOpen { get; }
	}

	public interface ILine
	{
		string Line { get; }

		SpecialCommands SpecialCommand { get; }
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

