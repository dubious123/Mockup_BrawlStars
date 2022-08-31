using ServerCore;
using ServerCore.Managers;
using ServerCore.Packets;
using ServerCore.Packets.Client;
using System.Net.Sockets;
using UnityEngine;
using static ServerCore.Utils.Enums;

public class ServerSession : Session
{
	public override void OnConnected()
	{
		base.OnConnected();
		Debug.Log($"[client] connecting to {_socket.RemoteEndPoint} completed");
		C_Chat packet = new C_Chat
		{
			Id = (ushort)PacketId.C_Chat
		};
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
			PacketHandler.HandlePacket(_recvBuffer.ReadPacket());
		}
		RegisterRecv();
	}

	public override bool RegisterSend(BasePacket packet)
	{
		return _sendBuffer.WritePacket(packet);
	}
}
