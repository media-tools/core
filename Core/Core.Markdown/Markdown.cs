using System;
using System.Threading.Tasks;
using CommonMark;

namespace Core.Markdown
{
	public static class Markdown
	{
		public static async Task<string> MarkdownToHTML (string markdownCode)
		{
			string htmlCode = null;
			await Task.Run (() => {
				CommonMarkSettings settings = CommonMarkSettings.Default;
				settings.OutputFormat = OutputFormat.Html;
				settings.AdditionalFeatures = CommonMarkAdditionalFeatures.All;
				htmlCode = CommonMarkConverter.Convert (source: markdownCode, settings: settings);
			});
			return htmlCode;
		}
	}
}

