using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Shell.Common
{
	public class Parser
	{
		internal string[] LineSeperators = new string[] { "\n", "\r\n", ";" };

		public ScriptBlock Result { get; private set; }

		public Parser ()
		{
			Result = new ScriptBlock ();
		}

		public void Clear ()
		{
			Result = new ScriptBlock ();
		}

		public void Parse (string script)
		{
			Result.Eat (ref script);
		}
	}

}

