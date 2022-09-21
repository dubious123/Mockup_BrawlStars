
using ServerCore;
using System;
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
public class C_BroadcastPlayerState : GamePacket
{
	public C_BroadcastPlayerState(int userId)
	{
		Id = 0x0004;
		UserId = userId;
	}
	public short TeamId;
	public float PosX;
	public float PosY;
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
	public int RoomId;
	public Vector2[] PlayerPosArr;
	public Vector2[] PlayerLookDirArr;

	public ushort PlayerCount;


}
