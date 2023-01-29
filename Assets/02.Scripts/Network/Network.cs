using System.Net;

using MEC;

using ServerCore;
using ServerCore.Managers;

using UnityEngine;

public class Network : MonoBehaviour
{
	private ServerSession _session;
	private Connector _connector;
	private static Network _instance;
	public static int RTT;
	public static int Latency => RTT / 2;
	public static bool Connected => _instance._session is not null;

	private CoroutineHandle _syncTimeHandle;

	public static void Init()
	{
		_instance = GameObject.Find("@Network").GetComponent<Network>();
		_instance._connector = new Connector(socket =>
		{
			_instance._session = SessionMgr.GenerateSession<ServerSession>(socket);
			return _instance._session;
		});

		var endPoint = new IPEndPoint(Config.CONNECT_ADDRESS, 7777);
		_instance._connector.StartConnect(endPoint);
		Debug.Log($"Connecting to {endPoint}");
	}

	public static void RegisterSend(BasePacket packet)
	{
		_instance._session.RegisterSend(packet);
	}

	private void OnApplicationQuit()
	{
		SessionMgr.CloseAll();
	}
}
