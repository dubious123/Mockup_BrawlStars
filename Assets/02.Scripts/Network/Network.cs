using ServerCore;
using ServerCore.Managers;
using UnityEngine;
using static ServerCore.Utils.Tools;

public class Network : MonoBehaviour
{
	ServerSession _session;
	Connector _connector;
	static Network _instance;
	void Start()
	{
		_instance = GameObject.Find("@Network").GetComponent<Network>();
		_instance._connector = new Connector(socket =>
		{
			_instance._session = SessionMgr.GenerateSession<ServerSession>(socket);
			return _instance._session;
		});
		var endPoint = GetNewEndPoint(7777);
		_instance._connector.StartConnect(endPoint);
		Debug.Log($"Connecting to {endPoint}");
	}
	private void Update()
	{

	}
	private void OnApplicationQuit()
	{
		SessionMgr.CloseAll();
	}

	public static void RegisterSend(BasePacket packet)
	{
		PacketQueue.Push(() =>
		{
			_instance._session.RegisterSend(packet);
		});

	}
}
