using System;
using Core.IO.Streams;

namespace Core.IO.Terminal
{
	public interface ITerminalStream : ITerminalInputStream, IFlexibleOutputStream
	{
	}
}

