﻿using System;
using System.IO;

using Server.Game.Data;

using UnityEngine;

public class Config
{
#if CLIENT
	public static string Id => _instance._configData.ID;
	public static string Pw => _instance._configData.PW;
#endif
	public static int MAX_PLAYER_COUNT => _instance._configData.MAX_PLAYER_COUNT;
	public static int MAX_ROUND_COUNT => _instance._configData.MAX_ROUND_COUNT;
	public static int TEAM_MEMBER_COUNT => _instance._configData.TEAM_MEMBER_COUNT;//3;
	public static int REQUIRED_WIN_COUNT => _instance._configData.REQUIRED_WIN_COUNT;
	public static int ROUND_END_WAIT_FRAMECOUNT => _instance._configData.ROUND_END_WAIT_FRAMECOUNT;
	public static int ROUND_CLEAR_WAIT_FRAMECOUNT => _instance._configData.ROUND_CLEAR_WAIT_FRAMECOUNT;
	public static int ROUND_RESET_WAIT_FRAMECOUNT => _instance._configData.ROUND_RESET_WAIT_FRAMECOUNT;
	public static int MAX_FRAME_COUNT => _instance._configData.MAX_FRAME_COUNT;//60 * 60 * 3;

	private static Config _instance;
	private ConfigData _configData;

	private Config()
	{
		var str = File.ReadAllText(Application.dataPath + "/../config.json");
		_configData = JsonUtility.FromJson<ConfigData>(str);
	}

	public static void Init() => _instance = new();

	[Serializable]
	private class ConfigData
	{
#if CLIENT
		public string ID;
		public string PW;
#endif
		public int MAX_PLAYER_COUNT;
		public int MAX_ROUND_COUNT;
		public int TEAM_MEMBER_COUNT;
		public int REQUIRED_WIN_COUNT;
		public int ROUND_END_WAIT_FRAMECOUNT;
		public int ROUND_CLEAR_WAIT_FRAMECOUNT;
		public int ROUND_RESET_WAIT_FRAMECOUNT;
		public int MAX_FRAME_COUNT;
	}
}
