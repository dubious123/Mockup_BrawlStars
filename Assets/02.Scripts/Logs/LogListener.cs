using System;
using UnityEngine;
using static Enums;
using System.Collections.Concurrent;

namespace Logging
{
	internal abstract class LogListener
	{
		public LogLevel Level { get; init; }
		Action _onLog;
		protected static string[] _header = new string[] { "Info", "Warning", "Error" };
		protected ConcurrentQueue<string> _logQueue;
		public LogListener(LogLevel level, LogOptionFlag option)
		{
			Level = level;
			_logQueue = new ConcurrentQueue<string>();
			_logQueue.Enqueue($"Level : [{_header[(int)level]}]");
			if (option.HasFlag(LogOptionFlag.DateTime))
			{
				_onLog += () => _logQueue.Enqueue($"DateTime : [{DateTime.Now}]");
			}
			if (option.HasFlag(LogOptionFlag.Callstack))
			{
				_onLog += () => _logQueue.Enqueue($"Callstack : {StackTraceUtility.ExtractStackTrace()}");
			}
		}
		public void Write(string message)
		{
			_onLog?.Invoke();
			_logQueue.Enqueue($"{message}");
		}
		public abstract void Flush();
	}
}

