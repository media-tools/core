using NUnit.Framework;
using System;
using Core.Shell.Common;
using Core.Common;
using Core.Platform;

namespace Core.UnitTests.Shell
{
	[TestFixture ()]
	public class Test
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
		public void TestCase ()
		{
			string code, expectedOutput;

			code = @"
				echo -n test1;echo test2
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
			expectedOutput = "test1test2\ntest7 a b c d   c abc\ntest4\n";
			Assert.AreEqual (expected: expectedOutput, actual: captureOutput (code), message: "code: " + expectedOutput);
			
		}

		private string captureOutput (string code)
		{
			string result = "";

			UnixShell shell = new UnixShell ();
			shell.Environment.Output.PipeTo (async line => result += line);
			shell.Environment.Error.PipeTo (async line => result += line);

			try {
				shell.RunScript (code: code);
			} catch (Exception ex) {
				Log.Error (ex);
			}

			result = result.Replace (Environment.NewLine, "\n");

			return result;
		}
	}
}

