using Logging;
using ServerCore;
using ServerCore.Managers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using static Enums;

public class ServerSession : Session
{
	JobQueue _sendQueue;
	JobQueue _parserQueue;
	ConcurrentQueue<BasePacket> _sendingPacketQueue;
	IEnumerator<float> _coPacketParserHandler;
	int _sendRegistered;
	public bool ParsingPacket;

	public override void Init(int id, Socket socket)
	{
		base.Init(id, socket);
		_sendRegistered = 0;
		_sendingPacketQueue = new ConcurrentQueue<BasePacket>();
		_sendQueue = JobMgr.GetQueue("PacketSend");
		_parserQueue = JobMgr.GetQueue("PacketParser");
		_coPacketParserHandler = _recvBuffer.ReadPacket(this);
	}
	public override void OnConnected()
	{
		base.OnConnected();
		JobMgr.PushUnityJob(() =>
		Debug.Log($"[client] connecting to {_socket.RemoteEndPoint} completed"));
		RegisterSend(new C_Init());
		Send();
	}
	protected override void Send()
	{
		try
		{
			base.Send();
		}
		catch (ObjectDisposedException e)
		{
			JobMgr.PushUnityJob(() => LogMgr.Log(LogSourceType.Session, LogLevel.Error, $"Session [{Id}] : {e.Message}"));
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
		_sendQueue.Push(() =>
		{
			while (_sendingPacketQueue.TryDequeue(out BasePacket item))
			{
				_sendBuffer.WritePacket(item);
			}
			_sendQueue.Push(() => Send());
		});
	}
	protected override void OnRecvCompleted(SocketAsyncEventArgs args)
	{
		base.OnRecvCompleted(args);
		if (args.BytesTransferred == 0)
		{
			SessionMgr.Close(Id);
			return;
		}
		_parserQueue.Push(() =>
		{
			_coPacketParserHandler.MoveNext();
			//todo
			//만약 한 악성 클라이언트가 엄청 빠르게 많이 보내면 queue가 해당 클라이언트의 패킷만 처리하면서 막힐 수 있다.
			RegisterRecv();
		});
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
			_sendQueue.Push(() => Send());
		}
		return result;
	}
}
