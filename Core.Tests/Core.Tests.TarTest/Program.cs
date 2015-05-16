using System;
using Core.Tar;
using Core.IO;
using Core.Common;

namespace Core.Tests.TarTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Logging.Enable ();

			Log.Info ("Hello World!");
			string encoded = TarIO.Write (TarIO.File.FromString ("test.txt", "fuck\r\nfuck"));
			Log.Info ("encoded: ", encoded);

			string reencoded = TarIO.StringEncoding.Encode (TarIO.StringEncoding.Decode (encoded));
			Log.Info ("reencoded: ", reencoded);

			Logging.Finish ();
		}
	}
}
