using System.IO;

namespace Core.IO.Streams
{
	public static class FlexibleStreamExtensions
	{
		public static IFlexibleStream ToFlexiblePipeTarget (StreamWriter streamWriter)
		{
			return new FlexibleStreamWriter (streamWriter);
		}
	}
}
