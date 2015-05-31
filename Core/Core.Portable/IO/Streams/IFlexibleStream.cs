using System.Threading.Tasks;

namespace Core.IO.Streams
{
	public interface IFlexibleStream
	{
		Task WriteAsync (string str);

		Task TryClose ();
	}
}

