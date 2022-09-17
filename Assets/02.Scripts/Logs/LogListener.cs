using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

namespace Logging
{
	internal abstract class LogListener
	{
		public LogLevel Level { get; init; }
		Action _onLog;
		protected string _log;
		protected static string[] _header;
		public LogListener(LogLevel level, LogOptionFlag option)
		{
			Level = level;
			_header = new string[] { "Info", "Warning", "Error" };
			_onLog = default(Action);
			_onLog += () => _log += $"Level : [{_header[(int)level]}]\n";
			if (option.HasFlag(LogOptionFlag.DateTime))
			{
				_onLog += () => _log += $"DateTime : [{DateTime.Now}]\n";
			}
			if (option.HasFlag(LogOptionFlag.Callstack))
			{
				_onLog += () => _log += "Callstack : " + StackTraceUtility.ExtractStackTrace() + "\n";
			}
		}
		public void Write(string message)
		{
			_onLog.Invoke();
			_log += message + "\n";
		}
		public abstract void Flush();
	}
}

