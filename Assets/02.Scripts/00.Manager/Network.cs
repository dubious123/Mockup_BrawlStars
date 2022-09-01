using ServerCore;
using ServerCore.Managers;
using UnityEngine;
using static ServerCore.Utils.Tools;

public class Network : MonoBehaviour
{
	void Start()
	{
		Connector connector = new Connector(socket => SessionMgr.GenerateSession<ServerSession>(socket));
		var endPoint = GetNewEndPoint(7777);
		connector.StartConnect(endPoint);
		Debug.Log($"Connecting to {endPoint}");
	}
	private void OnApplicationQuit()
	{
		SessionMgr.CloseAll();
	}
}
