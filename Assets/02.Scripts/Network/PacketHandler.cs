using ServerCore;
using ServerCore.Packets;
using System;
using System.Collections.Concurrent;
using static Enums;
using static ServerCore.Utils.Enums;
using UnityEngine;

public static class PacketHandler
{
	static ConcurrentDictionary<PacketId, Action<BasePacket, Session>> _handlerDict;
	#region Scenes
	static Scene_Login _login;
	static Scene_Login SceneLogin
	{
		get
		{
			if (_login == null)
			{
				_login = GameObject.Find("SceneLogin").GetComponent<Scene_Login>();
			}
			return _login;
		}
	}
	#endregion

	static PacketHandler()
	{
		_handlerDict = new ConcurrentDictionary<PacketId, Action<BasePacket, Session>>();
		_handlerDict.TryAdd(PacketId.S_Init, (packet,session) => PacketQueue.Push(() => S_InitHandle(packet, session)));
		_handlerDict.TryAdd(PacketId.S_Login, (packet,session) => PacketQueue.Push(() => S_LoginHandle(packet, session)));
		_handlerDict.TryAdd(PacketId.S_EnterLobby, (packet,session) => PacketQueue.Push(() => S_EnterLobbyHandle(packet, session)));
		_handlerDict.TryAdd(PacketId.S_EnterGame, (packet,session) => PacketQueue.Push(() => S_EnterGameHandle(packet, session)));
		_handlerDict.TryAdd(PacketId.S_BroadcastGameState, (packet,session) => PacketQueue.Push(() => S_BroadcastGameStateHandle(packet, session)));
	}

	public static void HandlePacket(BasePacket packet, Session session)
	{
		if (packet == null) return;
		if (_handlerDict.TryGetValue((PacketId)packet.Id, out Action<BasePacket, Session> action) == false)
			throw new Exception();
		action.Invoke(packet, session);
	}

	private static void S_InitHandle(BasePacket packet, Session session)
	{
		var req = packet as S_Init;
		Scene.MoveTo(SceneType.Login, null);
	}

	private static void S_LoginHandle(BasePacket packet, Session session)
	{
		var req = packet as S_Login;
		if (req.result == false)
		{
			SceneLogin.OnLoginFailed();
			return;
		}
		SceneLogin.OnLoginSuccess(req);
	}

	private static void S_EnterLobbyHandle(BasePacket packet, Session session)
	{
		var req = packet as S_EnterLobby;
		Scene.MoveTo(SceneType.Lobby, CharacterType.Dog);
	}

	private static void S_EnterGameHandle(BasePacket packet, Session session)
	{
		var req = packet as S_EnterGame;
		if (req.Result == false) return;
		User.GameRoomId = req.RoomId;
		Scene.MoveTo(SceneType.Game, User.CharType);
	}





	private static void S_BroadcastGameStateHandle(BasePacket packet, Session session)
	{
		var req = packet as S_BroadcastGameState;
	}
}
