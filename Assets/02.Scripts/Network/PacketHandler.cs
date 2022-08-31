using ServerCore;
using ServerCore.Packets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using static Enums;
using static ServerCore.Utils.Enums;

public static class PacketHandler
{
	static ConcurrentDictionary<PacketId, Action<BasePacket, Session>> _handlerDict;
	static PacketHandler()
	{
		_handlerDict = new ConcurrentDictionary<PacketId, Action<BasePacket, Session>>();
		_handlerDict.TryAdd(PacketId.S_Chat, (packet, session) => S_ChatHandle(packet, session));
		_handlerDict.TryAdd(PacketId.S_EnterLobby, (packet, session) => S_EnterLobbyHandle(packet, session));
	}

	public static void HandlePacket(BasePacket packet, Session session)
	{
		if (packet == null) return;
		if (_handlerDict.TryGetValue((PacketId)packet.Id, out Action<BasePacket, Session> action) == false)
			throw new Exception();
		action.Invoke(packet, session);
	}

	private static void S_ChatHandle(BasePacket packet, Session session)
	{
		packet = packet as S_Chat;
	}

	private static void S_EnterLobbyHandle(BasePacket packet, Session session)
	{
		packet = packet as S_EnterLobby;
		Scene.MoveTo(SceneType.Lobby, CharacterType.Dog);
	}
}
