using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Logging;

using UnityEngine;

using static Enums;

public class LogMgr : MonoBehaviour
{
	private static LogMgr _instance;

	[SerializeField] private string _dirPath;
	private LogSource[] _loggers;
	private List<IDisposable> _disposables;
	private bool _available;

	private void Awake()
	{
		_available = false;
		_instance = this;
		_dirPath = Application.dataPath + "/02.Scripts/Logs/";

		Directory.CreateDirectory(_dirPath);
		_loggers = new LogSource[10];
		_loggers[(int)LogSourceType.Console] = new LogSource()
			.AddConsoleListener(LogLevel.Info, LogOptionFlag.DateTime);
		var packetLogWriter = new FileStream(_dirPath + "Packet.txt", FileMode.Create, FileAccess.Write);
		var recvPacketLogWriter = new FileStream(_dirPath + "RecvPacket.txt", FileMode.Create, FileAccess.Write);
		var sendPacketLogWriter = new FileStream(_dirPath + "SendPacket.txt", FileMode.Create, FileAccess.Write);
		var debugLogWriter = new FileStream(_dirPath + "Debug.txt", FileMode.Create, FileAccess.Write);
		var gameLogWriter = new FileStream(_dirPath + "Game.txt", FileMode.Create, FileAccess.Write);
		var packetListener = new TextWriterLogListener(LogLevel.Info, LogOptionFlag.DateTime, packetLogWriter);

		_loggers[(int)LogSourceType.PacketRecv] = new LogSource()
			.AddListener(packetListener)
			.AddTextWriterLogListener(LogLevel.Info, LogOptionFlag.DateTime, recvPacketLogWriter);
		_loggers[(int)LogSourceType.PacketSend] = new LogSource()
			.AddListener(packetListener)
			.AddTextWriterLogListener(LogLevel.Info, LogOptionFlag.DateTime, sendPacketLogWriter);
		_loggers[(int)LogSourceType.Debug] = new LogSource()
			.AddTextWriterLogListener(LogLevel.Info, LogOptionFlag.DateTime, debugLogWriter);
		_loggers[(int)LogSourceType.Game] = new LogSource()
			.AddTextWriterLogListener(LogLevel.Info, LogOptionFlag.DateTime, gameLogWriter);
		_disposables = new()
		{
			packetLogWriter,
			recvPacketLogWriter,
			sendPacketLogWriter,
			debugLogWriter,
			gameLogWriter,
		};

		_available = true;
	}
	private void Update()
	{

	}

	private void OnApplicationQuit()
	{
		foreach (var logger in _loggers)
		{
			logger?.Flush();
		}
		foreach (var d in _disposables)
		{
			d.Dispose();
		}
	}

	public static void Log(LogSourceType type, LogLevel level, string message)
	{
		if (_instance._available == false) return;
		JobMgr.PushUnityJob(() => _instance._loggers[(int)type].Log(level, message));

	}

	public static void Log(LogSourceType type, params string[] messages)
	{
		if (_instance._available == false) return;
		JobMgr.PushUnityJob(() =>
		{
			foreach (var message in messages)
			{
				_instance._loggers[(int)type].Log(LogLevel.Info, message);
			}
		});
	}
}
