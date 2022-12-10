using System.Collections.Generic;
using System.IO;

using Serilog;

using UnityEngine;

using Logger = Serilog.Core.Logger;

public class Loggers : MonoBehaviour
{
	public static Logger Debug;
	public static Logger Game;
	public static Logger Send;
	public static Logger Recv;
	public static Logger Network;
	public static Logger Error;

	private static List<Logger> _loggers;

	public static void Init()
	{
		var path = Application.dataPath + "/02.Scripts/Logs/";

		File.WriteAllText(path + "/Debug.txt", "");
		File.WriteAllText(path + "/Game.txt", "");
		File.WriteAllText(path + "/Send.txt", "");
		File.WriteAllText(path + "/Recv.txt", "");
		File.WriteAllText(path + "/Network.txt", "");
		File.WriteAllText(path + "/Error.txt", "");

		Debug = new LoggerConfiguration().WriteTo.File(path + "/Debug.txt").CreateLogger();
		Game = new LoggerConfiguration().WriteTo.File(path + "/Game.txt").CreateLogger();
		Send = new LoggerConfiguration().WriteTo.File(path + "/Send.txt").CreateLogger();
		Recv = new LoggerConfiguration().WriteTo.File(path + "/Recv.txt").CreateLogger();
		Network = new LoggerConfiguration().WriteTo.File(path + "/Network.txt").CreateLogger();
		Error = new LoggerConfiguration().WriteTo.File(path + "/Error.txt").CreateLogger();

		_loggers = new() { Debug, Game, Send, Recv, Network, Error, };
	}

	private void OnApplicationQuit()
	{
		foreach (var logger in _loggers)
		{
			logger.Dispose();
		}

		Log.CloseAndFlush();
	}
}
