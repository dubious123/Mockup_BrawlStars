using ServerCore;
using ServerCore.Packets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using static ServerCore.Utils.Enums;
using UnityEngine;
using System.Text;

public static class PacketParser
{
	static readonly ConcurrentDictionary<ushort, Func<string, BasePacket>> _readDict;
	static PacketParser()
	{
		_readDict = new ConcurrentDictionary<ushort, Func<string, BasePacket>>();
		_readDict.TryAdd((ushort)PacketId.C_Init, json => JsonUtility.FromJson<C_Init>(json));
		_readDict.TryAdd((ushort)PacketId.C_Login, json => JsonUtility.FromJson<C_Login>(json));
		_readDict.TryAdd((ushort)PacketId.C_EnterLobby, json => JsonUtility.FromJson<C_EnterLobby>(json));
		_readDict.TryAdd((ushort)PacketId.C_EnterGame, json => JsonUtility.FromJson<C_EnterGame>(json));
		_readDict.TryAdd((ushort)PacketId.C_BroadcastPlayerState, json => JsonUtility.FromJson<C_BroadcastPlayerState>(json));
		_readDict.TryAdd((ushort)PacketId.S_Init, json => JsonUtility.FromJson<S_Init>(json));
		_readDict.TryAdd((ushort)PacketId.S_Login, json => JsonUtility.FromJson<S_Login>(json));
		_readDict.TryAdd((ushort)PacketId.S_EnterLobby, json => JsonUtility.FromJson<S_EnterLobby>(json));
		_readDict.TryAdd((ushort)PacketId.S_EnterGame, json => JsonUtility.FromJson<S_EnterGame>(json));
		_readDict.TryAdd((ushort)PacketId.S_BroadcastGameState, json => JsonUtility.FromJson<S_BroadcastGameState>(json));
	}
	public static BasePacket ReadPacket(this RecvBuffer buffer)
	{
		try
		{
			var id = BitConverter.ToUInt16(buffer.Read(2));
			var size = BitConverter.ToUInt16(buffer.Read(2));
			_readDict.TryGetValue(id, out Func<string, BasePacket> func);
			return func.Invoke(Encoding.UTF8.GetString(buffer.Read(size)));
		}
		catch (System.Exception)
		{
			return null;
		}
	}
	public static bool WritePacket(this SendBuffer buffer, BasePacket packet)
	{
		try
		{
			BitConverter.TryWriteBytes(buffer.Write(2), packet.Id);
			var json = new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonUtility.ToJson(packet)));
			Debug.Log(JsonUtility.ToJson(packet));
			BitConverter.TryWriteBytes(buffer.Write(2), (ushort)json.Count);
			json.CopyTo(buffer.Write(json.Count));
			return true;
		}
		catch (System.Exception)
		{
			return false;
		}
	}
}
