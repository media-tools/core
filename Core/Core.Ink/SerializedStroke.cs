using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Math;

namespace Core.Ink
{
	public class SerializedStroke
	{
		[JsonProperty ("segments")]
		public List<SerializedStrokeSegment> Segments = new List<SerializedStrokeSegment> ();

		public void Add (SerializedStrokeSegment serializedStrokeSegment)
		{
			Segments.Add (serializedStrokeSegment);
		}
	}

	public class SerializedStrokeSegment
	{
		[JsonProperty ("bezier_1")]
		public PortablePoint BezierControlPoint1 { get; set; }

		[JsonProperty ("bezier_2")]
		public PortablePoint BezierControlPoint2 { get; set; }

		[JsonProperty ("position")]
		public PortablePoint Position { get; set; }

		[JsonProperty ("pressure")]
		public float Pressure { get; set; }
	}
}
