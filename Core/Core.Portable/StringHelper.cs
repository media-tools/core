using System;

namespace Core.Common
{
	public static class StringHelper
	{
		public static string Base64Encode (string str)
		{
			return Convert.ToBase64String (System.Text.Encoding.UTF8.GetBytes (str));
		}

		public static string Base64Decode (string str)
		{
			byte[] buffer = Convert.FromBase64String (str);
			return System.Text.Encoding.UTF8.GetString (buffer, 0, buffer.Length);
		}
	}
}

