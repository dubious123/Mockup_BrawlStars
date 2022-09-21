using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

namespace Logging
{
	internal class ConsoleLogListener : LogListener
	{
		public ConsoleLogListener(LogLevel level, LogOptionFlag option) : base(level, option)
		{
		}
		public override void Flush()
		{
			while (_logQueue.TryDequeue(out var log))
			{
				Debug.Log(log);
			}
		}
	}
}

