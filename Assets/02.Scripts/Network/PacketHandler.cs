using ServerCore;
using ServerCore.Packets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using static ServerCore.Utils.Enums;

public static class PacketHandler
{
	static ConcurrentDictionary<PacketId, Action<BasePacket>> _handlerDict;
	static PacketHandler()
	{
		_handlerDict = new ConcurrentDictionary<PacketId, Action<BasePacket>>();
		_handlerDict.TryAdd(PacketId.S_Chat, packet => S_ChatHandle(packet));
	}

	public static void HandlePacket(BasePacket packet)
	{
		if (packet == null) return;
		if (_handlerDict.TryGetValue((PacketId)packet.Id, out Action<BasePacket> action) == false)
			throw new Exception();
		action.Invoke(packet);
	}

	private static void S_ChatHandle(BasePacket packet)
	{
		packet = packet as S_Chat;
	}
}
