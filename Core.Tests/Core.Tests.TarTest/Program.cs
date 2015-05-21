﻿using System;
using Core.Tar;
using Core.IO;
using Core.Common;
using System.Text;

namespace Core.Tests.TarTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Logging.Enable ();

			const string test = "Ein Test-Satz 123!\"§$$$$$$$%%%%%%%%%%%&/()=;:_!";
			Log.Info ("original: ", test);
			string testDeEncoded = Encoding.UTF8.GetString (ReadableEncoding.Decode (ReadableEncoding.Encode (Encoding.UTF8.GetBytes (test))));
			Log.Info ("testDeEncoded: ", testDeEncoded);
			string testDeDeEnEncoded = Encoding.UTF8.GetString (ReadableEncoding.Decode (Encoding.UTF8.GetString (ReadableEncoding.Decode (
				                           ReadableEncoding.Encode (Encoding.UTF8.GetBytes (ReadableEncoding.Encode (Encoding.UTF8.GetBytes (test))))))));
			Log.Info ("testDeDeEnEncoded: ", testDeDeEnEncoded);


			Log.Info ("Hello World!");
			string encoded = TarIO.WriteString (TarIO.File.FromString ("test.txt", "fuck\r\nfuck"));
			Log.Info ("encoded: ", encoded);

			string reencoded = ReadableEncoding.Encode (ReadableEncoding.Decode (encoded));
			Log.Info ("reencoded: ", reencoded);

			Logging.Finish ();
		}
	}
}