using ServerCore;
using ServerCore.Managers;
using UnityEngine;
using static ServerCore.Utils.Tools;

public class Network : MonoBehaviour
{
	void Start()
	{
		Connector connector = new Connector(socket => SessionMgr.GenerateSession<ServerSession>(socket));
		connector.StartConnect(GetNewEndPoint(7777));
		Debug.Log("Connecting...");
	}
	private void OnApplicationQuit()
	{
		SessionMgr.CloseAll();
	}
}
