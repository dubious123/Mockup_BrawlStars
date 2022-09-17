using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Enums
{
	public enum SceneType
	{
		Entry,
		Login,
		Lobby,
		Game
	}
	public enum CharacterType
	{
		Dog,
	}

	public enum LogSourceType
	{
		Packet,
		Network,
		Session,
		PacketHandle,
		Console,
		Error,
		PacketSend,
		PacketRecv,
	}
	public enum LogLevel
	{
		Info,
		Warning,
		Error,
	}
	[Flags]
	public enum LogOptionFlag
	{
		None = 0,
		DateTime = 1,
		Callstack = 2,
	}
}
