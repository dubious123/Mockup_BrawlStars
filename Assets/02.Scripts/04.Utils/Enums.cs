using System;

public static partial class Enums
{
	public enum SceneType
	{
		Entry,
		Login,
		Lobby,
		Game,
		Loading,
		SearchingPlayers,
	}

	public enum CharacterType
	{
		None,
		Knight,
		Shelly
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
		Debug,
		Game,
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
		FixedTime = 4,
	}
}
