using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

using ServerCore;

using UnityEngine;

using static ServerCore.Utils.Enums;

public static class PacketParser
{
	private static readonly ConcurrentDictionary<ushort, Func<string, BasePacket>> _readDict;
	private static JobQueue _packetHandlerQueue;
	static PacketParser()
	{
		_packetHandlerQueue = JobMgr.GetQueue("PacketHandler");
		_readDict = new ConcurrentDictionary<ushort, Func<string, BasePacket>>();
		_readDict.TryAdd((ushort)PacketId.C_Init, json => JsonUtility.FromJson<C_Init>(json));
		_readDict.TryAdd((ushort)PacketId.C_Login, json => JsonUtility.FromJson<C_Login>(json));
		_readDict.TryAdd((ushort)PacketId.C_EnterLobby, json => JsonUtility.FromJson<C_EnterLobby>(json));
		_readDict.TryAdd((ushort)PacketId.C_EnterGame, json => JsonUtility.FromJson<C_EnterGame>(json));
		_readDict.TryAdd((ushort)PacketId.C_GameReady, json => JsonUtility.FromJson<C_GameReady>(json));
		_readDict.TryAdd((ushort)PacketId.C_PlayerInput, json => JsonUtility.FromJson<C_PlayerInput>(json));
		_readDict.TryAdd((ushort)PacketId.S_Init, json => JsonUtility.FromJson<S_Init>(json));
		_readDict.TryAdd((ushort)PacketId.S_Login, json => JsonUtility.FromJson<S_Login>(json));
		_readDict.TryAdd((ushort)PacketId.S_EnterLobby, json => JsonUtility.FromJson<S_EnterLobby>(json));
		_readDict.TryAdd((ushort)PacketId.S_GameReady, json => JsonUtility.FromJson<S_GameReady>(json));
		_readDict.TryAdd((ushort)PacketId.S_EnterGame, json => JsonUtility.FromJson<S_EnterGame>(json));
		_readDict.TryAdd((ushort)PacketId.S_BroadcastSearchPlayer, json => JsonUtility.FromJson<S_BroadcastSearchPlayer>(json));
		_readDict.TryAdd((ushort)PacketId.S_BroadcastEnterGame, json => JsonUtility.FromJson<S_BroadcastEnterGame>(json));
		_readDict.TryAdd((ushort)PacketId.S_BroadcastStartGame, json => JsonUtility.FromJson<S_BroadcastStartGame>(json));
		_readDict.TryAdd((ushort)PacketId.S_GameFrameInfo, json => JsonUtility.FromJson<S_GameFrameInfo>(json));
	}

	public static IEnumerator<float> ReadPacket(this RecvBuffer buffer, ServerSession session)
	{
		while (true)
		{
			while (buffer.CanRead() == false)
			{
				yield return 0f;
			}

			ushort id = BitConverter.ToUInt16(buffer.Read(2));
			ushort size = BitConverter.ToUInt16(buffer.Read(2));
			while (buffer.CanRead(size) == false)
			{
				yield return 0f;
			}

			_readDict.TryGetValue(id, out var func);
			string json = Encoding.UTF8.GetString(buffer.Read(size));
			Loggers.Recv.Information(json);
			var packet = func.Invoke(json);
			_packetHandlerQueue.Push(() => PacketHandler.HandlePacket(packet, session));
		}
	}

	public static bool WritePacket(this SendBuffer buffer, BasePacket packet)
	{
		try
		{
			BitConverter.TryWriteBytes(buffer.Write(2), packet.Id);
			var json = new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonUtility.ToJson(packet)));
			BitConverter.TryWriteBytes(buffer.Write(2), (ushort)json.Count);
			Loggers.Send.Information("size : [{0}] \n{1}", (ushort)json.Count, JsonUtility.ToJson(packet, true));
			json.CopyTo(buffer.Write(json.Count));
			return true;
		}
		catch (System.Exception)
		{
			throw new Exception();
		}
	}
}
