using ServerCore;
using ServerCore.Managers;
using ServerCore.Packets;
using System.Net.Sockets;
using UnityEngine;

public class ServerSession : Session
{
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
		return _sendBuffer.WritePacket(packet);
	}
}
