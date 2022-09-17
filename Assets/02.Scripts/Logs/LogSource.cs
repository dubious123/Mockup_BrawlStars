using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Enums;
using System.IO;

namespace Logging
{
	internal class LogSource
	{
		List<LogListener>[] _listenerList;
		internal LogSource()
		{
			_listenerList = new List<LogListener>[3] { new(), new(), new() };
		}
		/// <summary>
		/// log level error => info, warning, error
		/// log level warning => info, warning
		/// log level info => info
		/// </summary>
		/// <param name="level"></param>
		/// <param name="message"></param>
		internal void Log(LogLevel level, string message)
		{
			var max = (int)level;
			for (int i = 0; i <= max; i++)
			{
				foreach (var listener in _listenerList[i])
				{
					listener.Write(message);
				}
			}
		}
		internal void Flush()
		{
			foreach (var arr in _listenerList)
			{
				foreach (var l in arr)
				{
					l.Flush();
				}
			}
		}
		internal LogSource AddListener(LogListener listener)
		{
			_listenerList[(int)listener.Level].Add(listener);
			return this;
		}
		internal LogSource AddConsoleListener(LogLevel level, LogOptionFlag flag)
		{
			return AddListener(new ConsoleLogListener(level, flag));
		}
		internal LogSource AddTextWriterLogListener(LogLevel level, LogOptionFlag flag, FileStream writer)
		{
			return AddListener(new TextWriterLogListener(level, flag, writer));
		}
	}
}

