using System;
using Newtonsoft.Json;
using Core.Shell.Common.FileSystems;

namespace Core.Media.Common
{
	public abstract class Medium
	{
		[JsonProperty ("file")]
		public VirtualFile File { get; set; } = null;

		public string Filename { get { return File?.Path?.FileName; } }

		[JsonProperty ("url_hosted")]
		public string HostedURL { get; set; } = null;

		[JsonProperty ("mime_type")]
		public string MimeType { get; set; } = null;

		[JsonProperty ("timestamp_local")]
		public DateTime? DateTimeLocal { get; set; } = null;

		[JsonProperty ("timestamp_utc")]
		public DateTime? DateTimeUTC { get; set; } = null;

		protected Medium ()
		{
		}
	}
}

