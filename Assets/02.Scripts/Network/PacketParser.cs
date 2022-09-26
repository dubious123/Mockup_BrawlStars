using ServerCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using static ServerCore.Utils.Enums;
using UnityEngine;
using System.Text;
using Logging;

public static class PacketParser
{
	static readonly ConcurrentDictionary<ushort, Func<string, BasePacket>> _readDict;
	static JobQueue _packetHandlerQueue;
	static PacketParser()
	{
		_packetHandlerQueue = JobMgr.GetQueue("PacketHandler");
		_readDict = new ConcurrentDictionary<ushort, Func<string, BasePacket>>();
		_readDict.TryAdd((ushort)PacketId.C_Init, json => JsonUtility.FromJson<C_Init>(json));
		_readDict.TryAdd((ushort)PacketId.C_Login, json => JsonUtility.FromJson<C_Login>(json));
		_readDict.TryAdd((ushort)PacketId.C_EnterLobby, json => JsonUtility.FromJson<C_EnterLobby>(json));
		_readDict.TryAdd((ushort)PacketId.C_EnterGame, json => JsonUtility.FromJson<C_EnterGame>(json));
		_readDict.TryAdd((ushort)PacketId.C_GameReady, json => JsonUtility.FromJson<C_GameReady>(json));
		_readDict.TryAdd((ushort)PacketId.C_BroadcastPlayerInput, json => JsonUtility.FromJson<C_BroadcastPlayerInput>(json));
		_readDict.TryAdd((ushort)PacketId.S_Init, json => JsonUtility.FromJson<S_Init>(json));
		_readDict.TryAdd((ushort)PacketId.S_Login, json => JsonUtility.FromJson<S_Login>(json));
		_readDict.TryAdd((ushort)PacketId.S_EnterLobby, json => JsonUtility.FromJson<S_EnterLobby>(json));
		_readDict.TryAdd((ushort)PacketId.S_EnterGame, json => JsonUtility.FromJson<S_EnterGame>(json));
		_readDict.TryAdd((ushort)PacketId.S_BroadcastEnterGame, json => JsonUtility.FromJson<S_BroadcastEnterGame>(json));
		_readDict.TryAdd((ushort)PacketId.S_BroadcastStartGame, json => JsonUtility.FromJson<S_BroadcastStartGame>(json));
		_readDict.TryAdd((ushort)PacketId.S_BroadcastGameState, json => JsonUtility.FromJson<S_BroadcastGameState>(json));
		_readDict.TryAdd((ushort)PacketId.S_BroadcastMove, json => JsonUtility.FromJson<S_BroadcastMove>(json));
	}
	public static BasePacket ReadPacket(this RecvBuffer buffer)
	{
		try
		{
			var id = BitConverter.ToUInt16(buffer.Read(2));
			var size = BitConverter.ToUInt16(buffer.Read(2));
			_readDict.TryGetValue(id, out Func<string, BasePacket> func);
			//return func.Invoke(Encoding.UTF8.GetString(buffer.Read(size)));
			var json = Encoding.UTF8.GetString(buffer.Read(size));
			LogMgr.Log(Enums.LogSourceType.PacketRecv, json);
			var ret = func.Invoke(json);
			LogMgr.Log(Enums.LogSourceType.PacketRecv, "after parse : \n" + JsonUtility.ToJson(ret, true));
			return ret;
		}
		catch (System.Exception)
		{
			return null;
		}
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
			//Todo handle invalid id
			_readDict.TryGetValue(id, out var func);
#if DEBUG
			string json = Encoding.UTF8.GetString(buffer.Read(size));
			LogMgr.Log(Enums.LogSourceType.PacketRecv, json);
			var packet = func.Invoke(json);
			//LogMgr.Log(Enums.LogSourceType.PacketRecv, "after parse : \n" + JsonUtility.ToJson(packet, true));
			_packetHandlerQueue.Push(() => PacketHandler.HandlePacket(packet, session));
#else
			_packetHandlerQueue.Push(() => PacketHandler.HandlePacket(func.Invoke(buffer.Read(size)), session));
#endif
		}
	}
	public static bool WritePacket(this SendBuffer buffer, BasePacket packet)
	{
		try
		{
			//var arr1 = buffer.Write(2);
			//BitConverter.TryWriteBytes(arr1, packet.Id);

			//var json = new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonUtility.ToJson(packet)));

			//var arr2 = buffer.Write(2);
			//BitConverter.TryWriteBytes(arr2, (ushort)json.Count);

			//LogMgr.Log(Enums.LogSourceType.PacketSend, $"size : [{(ushort)json.Count}]" + JsonUtility.ToJson(packet));
			//string str = string.Empty;

			//str += BitConverter.ToString(arr1.Array, arr1.Offset, arr1.Count);
			//str += BitConverter.ToString(arr2.Array, arr2.Offset, arr2.Count);
			//str += BitConverter.ToString(json.Array, json.Offset, json.Count);

			//LogMgr.Log(Enums.LogSourceType.Debug, str);
			//json.CopyTo(buffer.Write(json.Count));
			BitConverter.TryWriteBytes(buffer.Write(2), packet.Id);
			var json = new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonUtility.ToJson(packet)));
			BitConverter.TryWriteBytes(buffer.Write(2), (ushort)json.Count);
			LogMgr.Log(Enums.LogSourceType.PacketSend, $"size : [{(ushort)json.Count}] \n{JsonUtility.ToJson(packet)}");
			json.CopyTo(buffer.Write(json.Count));
			return true;
		}
		catch (System.Exception)
		{
			throw new Exception();
		}
	}
}
