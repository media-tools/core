using System.Threading.Tasks;

namespace Core.IO.Streams
{
	public interface IFlexibleOutputStream
	{
		Task WriteAsync (string str);

		Task TryClose ();
	}

	/*public interface IFlexibleBufferedOutputStream : IFlexibleOutputStream
	{
		Task WriteAsync (params object[] values);

		Task WriteLineAsync (params object[] values);
	}*/

	public interface IFlexibleInputStream
	{
		bool TryReadLine (out string line);
	}
}

