using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

using MEC;

using ServerCore;
using ServerCore.Managers;

using UnityEngine;

using static Enums;

public class ServerSession : Session
{
	//private JobQueue _sendQueue;
	//private JobQueue _parserQueue;
	private ConcurrentQueue<BasePacket> _sendingPacketQueue;
	private IEnumerator<float> _coPacketParserHandler;
	private int _sendRegistered;
	public bool ParsingPacket;

	public override void Init(int id, Socket socket)
	{
		base.Init(id, socket);
		_sendRegistered = 0;
		_sendingPacketQueue = new ConcurrentQueue<BasePacket>();
		//_sendQueue = JobMgr.GetQueue("PacketSend");
		//_parserQueue = JobMgr.GetQueue("PacketParser");
		_coPacketParserHandler = _recvBuffer.ReadPacket(this);
	}
	public override void OnConnected()
	{
		base.OnConnected();
		JobMgr.PushUnityJob(() => Debug.Log($"[client] connecting to {_socket.RemoteEndPoint} completed"));
		JobMgr.PushUnityJob(Network.SyncTime);
	}
	protected override void Send()
	{
		try
		{
			base.Send();
		}
		catch (ObjectDisposedException e)
		{
			Loggers.Error.Information("Session [{0}] : {1}", Id, e.Message);
#if UNITY_EDITOR
			JobMgr.PushUnityJob(() => Debug.Log($"Session [{Id}] : {e.Message}"));
#endif
		}
		catch (Exception)
		{
			throw;
		}
	}
	protected override void OnSendCompleted(SocketAsyncEventArgs args)
	{
		base.OnSendCompleted(args);
		if (_sendingPacketQueue.IsEmpty)
		{
			_sendRegistered = 0;
			return;
		}

		while (_sendingPacketQueue.TryDequeue(out BasePacket item))
		{
			_sendBuffer.WritePacket(item);
		}

		Send();
	}
	protected override void OnRecvCompleted(SocketAsyncEventArgs args)
	{
		base.OnRecvCompleted(args);
		if (args.BytesTransferred == 0)
		{
			SessionMgr.Close(Id);
			return;
		}

		_coPacketParserHandler.MoveNext();
		//todo
		//만약 한 악성 클라이언트가 엄청 빠르게 많이 보내면 queue가 해당 클라이언트의 패킷만 처리하면서 막힐 수 있다.
		RegisterRecv();
	}

	public override bool RegisterSend(BasePacket packet)
	{
		var result = true;
		_sendingPacketQueue.Enqueue(packet);
		if (Interlocked.CompareExchange(ref _sendRegistered, 1, 0) == 0)
		{
			while (_sendingPacketQueue.TryDequeue(out BasePacket item))
			{
				result &= _sendBuffer.WritePacket(item);
			}

			Send();
		}
		return result;
	}
}
