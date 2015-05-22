using NUnit.Framework;
using System;
using Core.Shell.Common;
using Core.Common;

namespace Core.UnitTests.Shell
{
	[TestFixture ()]
	public class Test
	{
		[TestFixtureSetUp ()]
		public void Setup ()
		{
			Core.IO.Logging.Enable ();
		}

		[TestFixtureTearDown ()]
		public void TearDown ()
		{
			Core.IO.Logging.Finish ();
		}

		[Test ()]
		public void TestCase ()
		{
			string code, expectedOutput;

			code = @"
				echo -n test;echo test2
				echo test7 a  ""b c d"" """" ''      'c' abc;
				if true;
				then
					echo test4;
				elif false ; then
					echo test5
				else ;;;
					echo test6; echo test7
				fi
			";
			expectedOutput = @"";
			Assert.AreEqual (expected: expectedOutput, actual: captureOutput (code), message: "code: " + expectedOutput);
			
		}

		private string captureOutput (string code)
		{
			string result = "";

			UnixShell shell = new UnixShell ();
			shell.Environment.Output.Stream = line => result += line;

			try {
				shell.RunScript (code: code);
			} catch (Exception ex) {
				Log.Error (ex);
			}

			return result;
		}
	}
}

