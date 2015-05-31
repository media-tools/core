using System.Threading.Tasks;

namespace Core.IO.Streams
{
	public interface IFlexibleOutputStream
	{
		Task WriteAsync (string str);

		Task TryClose ();
	}

	/*public interface IFlexibleInputStream
	{
		bool TryReadLine (out string line);
	}*/
}

