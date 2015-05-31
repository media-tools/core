using System;
using System.Threading.Tasks;
using Core.Common;
using Core.IO.Streams;
using Core.Platform;
using Core.Shell.Common;
using NUnit.Framework;

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
			var capture = new FlexibleCaptureStream ();

			UnixShell shell = new UnixShell ();
			shell.Environment.Output.PipeTo (capture);
			shell.Environment.Error.PipeTo (capture);

			try {
				Task.Run (async () => await shell.RunScriptAsync (code: code)).Wait ();
			} catch (Exception ex) {
				Log.Error (ex);
			}

			var result = capture.Result;
			result = result.Replace (Environment.NewLine, "\n");
			return result;
		}

		private class FlexibleCaptureStream : IFlexibleOutputStream
		{
			public string Result { get; private set; } = string.Empty;

			#region IFlexibleStream implementation

			public Task WriteAsync (string str)
			{
				Result += str;
				return TaskHelper.Completed;
			}

			public Task TryClose ()
			{
				return TaskHelper.Completed;
			}

			#endregion
		}
	}
}

