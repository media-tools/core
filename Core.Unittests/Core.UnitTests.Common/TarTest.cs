using NUnit.Framework;
using System;
using Core.Common;
using System.Text;
using Core.Tar;
using Core.Platform;

namespace Core.UnitTests.Common
{
	[TestFixture ()]
	public class TarTest
	{
		[TestFixtureSetUp ()]
		public void Setup ()
		{
			UnitTestPlatform.Start ();
		}

		[TestFixtureTearDown ()]
		public void TearDown ()
		{
			UnitTestPlatform.Finish ();
		}

		[Test ()]
		public void TestStringEncoding ()
		{
			const string test = "Ein Test-Satz 123!\"§$$$$$$$%%%%%%%%%%%&/()=;:_!";

			string testDeEncoded = Encoding.UTF8.GetString (ReadableEncoding.Decode (ReadableEncoding.Encode (Encoding.UTF8.GetBytes (test))));
			Assert.AreEqual (expected: test, actual: testDeEncoded, message: "testDeEncoded");

			string testDeDeEnEncoded = Encoding.UTF8.GetString (ReadableEncoding.Decode (Encoding.UTF8.GetString (ReadableEncoding.Decode (
				                           ReadableEncoding.Encode (Encoding.UTF8.GetBytes (ReadableEncoding.Encode (Encoding.UTF8.GetBytes (test))))))));
			Assert.AreEqual (expected: test, actual: testDeDeEnEncoded, message: "testDeDeEnEncoded");
		}

		[Test ()]
		public void TestTar ()
		{
			const string testFileName = "test.txt";
			const string testFileContent = "fuck\r\nfuck";
			string encoded = TarIO.WriteString (TarIO.File.FromString (name: testFileName, content: testFileContent));

			string reencoded = ReadableEncoding.Encode (ReadableEncoding.Decode (encoded));
			Assert.AreEqual (expected: encoded, actual: reencoded, message: "encoded-reencoded");

			TarIO.File[] files = TarIO.ReadString (encoded);
			Assert.AreEqual (expected: 1, actual: files.Length, message: "count TarIO.File[]");
			Assert.AreEqual (expected: testFileName, actual: files [0].Name, message: "TarIO.File.Name");
			Assert.AreEqual (expected: testFileContent, actual: Encoding.UTF8.GetString (files [0].BinaryContent), message: "TarIO.File.Name");

		}
	}
}

