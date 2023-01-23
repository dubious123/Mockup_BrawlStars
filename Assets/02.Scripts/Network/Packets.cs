
using System;
using System.Collections.Generic;

using ServerCore;

using UnityEngine;

public class AuthPacket : BasePacket
{
	public int UserId;
}

public class GamePacket : AuthPacket
{
	public int RoomId;
	public ushort CharacterType;
}

public class C_Init : BasePacket
{
	public C_Init()
	{
		Id = 0x0000;
	}
}
public class C_SyncTime : BasePacket
{
	public C_SyncTime()
	{
		Id = 0x0001;
	}

	public long ClientLocalTime;
}
public class C_Login : BasePacket
{
	public C_Login()
	{
		Id = 0x0002;
	}

	public string loginId;
	public string loginPw;
}
public class C_EnterLobby : AuthPacket
{
	public C_EnterLobby()
	{
		Id = 0x0003;
	}
}
public class C_EnterGame : AuthPacket
{
	public C_EnterGame()
	{
		Id = 0x0004;
	}

	public ushort CharacterType;
}
public class C_GameReady : AuthPacket
{
	public C_GameReady(int userId)
	{
		Id = 0x0005;
		UserId = userId;
	}
}
public class C_PlayerInput : GamePacket
{
	public C_PlayerInput(int userId, int frameNum, sVector2 moveDir, sVector2 lookDir, byte buttonPressed)
	{
		Id = 0x0006;
		UserId = userId;
		FrameNum = frameNum;
		MoveDirX = moveDir.x.RawValue;
		MoveDirY = moveDir.y.RawValue;
		LookDirX = lookDir.x.RawValue;
		LookDirY = lookDir.y.RawValue;
		ButtonPressed = buttonPressed;
	}

	public byte ButtonPressed;
	public short TeamId;
	public int FrameNum;
	public uint MoveDirX;
	public uint MoveDirY;
	public uint LookDirX;
	public uint LookDirY;
}
public class S_Init : BasePacket
{
}
public class S_SyncTime : BasePacket
{
	public long ClientLocalTime;
	public long ServerTime;
}
public class S_Login : BasePacket
{
	public bool result;
	public int userId;
}
public class S_EnterLobby : BasePacket
{
}
public class S_EnterGame : BasePacket
{

	[Serializable]
	public struct PlayerInfoDto
	{

		public ushort CharacterType;
	}

	public short TeamId;
	public PlayerInfoDto PlayerInfo;
}
public class S_BroadcastFoundPlayer : BasePacket
{

	public int FoundPlayersCount;
}
public class S_BroadcastEnterGame : BasePacket
{

	public ushort Charactertype;
	public short TeamId;
}
public class S_BroadcastStartGame : BasePacket
{

	public ushort[] CharacterTypeArr;
}
public class S_GameFrameInfo : BasePacket
{

	public int FrameNum;
	public uint[] PlayerMoveDirXArr;
	public uint[] PlayerMoveDirYArr;
	public uint[] PlayerLookDirXArr;
	public uint[] PlayerLookDirYArr;
	public ushort[] ButtonPressedArr;
}
public class S_BroadcastRoundEnd : BasePacket
{
}
public class S_BroadcastRoundClear : BasePacket
{
}
public class S_BroadcastRoundReset : BasePacket
{
}
public class S_BroadcastMatchOver : BasePacket
{
}
