using ServerCore;
using ServerCore.Managers;
using ServerCore.Packets;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class ServerSession : Session
{
	ConcurrentQueue<BasePacket> _sendingPacketQueue;
	int _sendRegistered;
	public override void Init(int id, Socket socket)
	{
		base.Init(id, socket);
		_sendRegistered = 0;
		_sendingPacketQueue = new ConcurrentQueue<BasePacket>();
	}
	public override void OnConnected()
	{
		base.OnConnected();
		Debug.Log($"[client] connecting to {_socket.RemoteEndPoint} completed");
		C_Init packet = new C_Init();
		RegisterSend(packet);
		Send();
	}
	protected override void OnSendCompleted(SocketAsyncEventArgs args)
	{
		base.OnSendCompleted(args);
		if (_sendingPacketQueue.IsEmpty)
		{
			_sendRegistered = 0;
			return;
		}
		PacketQueue.Push(() =>
		{
			while (_sendingPacketQueue.TryDequeue(out BasePacket item))
			{
				_sendBuffer.WritePacket(item);
			}
			PacketQueue.Push(() => Send());
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
		while (_recvBuffer.CanRead())
		{
			PacketHandler.HandlePacket(_recvBuffer.ReadPacket(), this);
		}
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
			PacketQueue.Push(() => Send());
		}
		return result;
	}
}
