using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;
using System.IO;
using System;
using System.Text;

namespace Logging
{
	internal class TextWriterLogListener : LogListener
	{
		FileStream _writer;
		public TextWriterLogListener(LogLevel level, LogOptionFlag option, FileStream writer) : base(level, option)
		{
			_writer = writer;
		}
		public override void Flush()
		{
			_writer.Write(Encoding.UTF8.GetBytes(_log));
			_writer.Flush();
			_log = string.Empty;
		}
	}
}

