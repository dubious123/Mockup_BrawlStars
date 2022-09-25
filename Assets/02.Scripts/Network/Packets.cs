
using System;
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
public class C_Login : BasePacket
{
	public C_Login()
	{
		Id = 0x0001;
	}
	public string loginId;
	public string loginPw;
}
public class C_EnterLobby : AuthPacket
{
	public C_EnterLobby()
	{
		Id = 0x0002;
	}
}
public class C_EnterGame : AuthPacket
{
	public C_EnterGame()
	{
		Id = 0x0003;
	}
	public ushort CharacterType;
}
public class C_BroadcastPlayerInput : GamePacket
{
	public C_BroadcastPlayerInput(int userId, long startTick, Vector2 moveInput, Vector2 lookInput)
	{
		Id = 0x0004;
		UserId = userId;
		StartTick = startTick;
		MoveDirX = moveInput.x;
		MoveDirY = moveInput.y;
		LookDirX = lookInput.x;
		LookDirY = lookInput.y;
	}
	public short TeamId;
	public long StartTick;
	public float MoveDirX;
	public float MoveDirY;
	public float LookDirX;
	public float LookDirY;
}
public class S_Init : BasePacket
{
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
	public PlayerInfoDto[] PlayerInfoArr;
}
public class S_BroadcastEnterGame : BasePacket
{
	public ushort Charactertype;
	public short TeamId;
}
public class S_BroadcastGameState : BasePacket
{
	public long StartTick;
	public long TargetTick;
	public Vector2[] PlayerMoveDirArr;
	public Vector2[] PlayerLookDirArr;


}
public class S_BroadcastMove : BasePacket
{
	public Vector2 MoveDir;
	public Vector2 LookDir;
	public short TeamId;
}
