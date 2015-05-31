using System.IO;
using System.Threading.Tasks;
using Core.Common;

namespace Core.IO.Streams
{
	public class FlexibleStreamWriter : IFlexibleOutputStream
	{
		readonly StreamWriter streamWriter;

		public bool IsDisposable { get; set; } = false;

		public FlexibleStreamWriter (StreamWriter streamWriter)
		{
			this.streamWriter = streamWriter;
		}

		#region IFlexibleStream implementation

		public async Task WriteAsync (string str)
		{
			await streamWriter.WriteAsync (str);
		}

		public Task TryClose ()
		{
			if (IsDisposable) {
				streamWriter.Dispose ();
			}
			return Actions.EmptyAsync ();
		}

		#endregion
	}

}
