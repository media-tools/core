using System;
using Newtonsoft.Json;
using Core.Shell.Common.FileSystems;
using Core.Math;

namespace Core.Media.Common
{
	public class PhotoDimensions
	{
		[JsonProperty ("width")]
		public int Width { get; set; } = 0;

		[JsonProperty ("height")]
		public int Height { get; set; } = 0;
	}
}
