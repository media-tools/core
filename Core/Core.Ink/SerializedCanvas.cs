using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Math;

namespace Core.Ink
{
	public class SerializedCanvasCollection
	{
		[JsonProperty ("pages")]
		public List<SerializedCanvas> Pages = new List<SerializedCanvas> ();

		public void Add (SerializedCanvas serializedCanvas)
		{
			Pages.Add (serializedCanvas);
		}
	}

	public class SerializedCanvas
	{
		[JsonProperty ("strokes")]
		public List<SerializedStroke> Strokes = new List<SerializedStroke> ();

		public void Add (SerializedStroke serializedStroke)
		{
			Strokes.Add (serializedStroke);
		}
	}
}
