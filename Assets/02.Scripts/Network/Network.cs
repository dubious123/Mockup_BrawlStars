using ServerCore;
using ServerCore.Managers;

using UnityEngine;

using static ServerCore.Utils.Tools;

public class Network : MonoBehaviour
{
	private ServerSession _session;
	private Connector _connector;
	private static Network _instance;

	public static bool Connected => _instance._session is not null;

	public static void Init()
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

	private void OnApplicationQuit()
	{
		SessionMgr.CloseAll();
	}

	public static void RegisterSend(BasePacket packet)
	{
		_instance._session.RegisterSend(packet);
	}
}
