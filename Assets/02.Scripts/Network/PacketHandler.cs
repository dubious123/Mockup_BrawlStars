using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Cryptography;

using MEC;

using ServerCore;

using UnityEngine;

using static Enums;
using static ServerCore.Utils.Enums;

public static class PacketHandler
{
	private static ConcurrentDictionary<PacketId, Action<BasePacket, ServerSession>> _handlerDict;

	static PacketHandler()
	{
		_handlerDict = new ConcurrentDictionary<PacketId, Action<BasePacket, ServerSession>>();
		_handlerDict.TryAdd(PacketId.S_Init, (packet, session) => S_InitHandle(packet, session));
		_handlerDict.TryAdd(PacketId.S_SyncTime, (packet, session) => S_SyncTimeHandle(packet, session));
		_handlerDict.TryAdd(PacketId.S_Login, (packet, session) => S_LoginHandle(packet, session));
		_handlerDict.TryAdd(PacketId.S_EnterLobby, (packet, session) => S_EnterLobbyHandle(packet, session));
		_handlerDict.TryAdd(PacketId.S_GameReady, (packet, session) => S_GameReadyHandle(packet, session));
		_handlerDict.TryAdd(PacketId.S_EnterGame, (packet, session) => S_EnterGameHandle(packet, session));
		_handlerDict.TryAdd(PacketId.S_BroadcastSearchPlayer, (packet, session) => S_BroadcastSearchPlayerHandle(packet, session));
		_handlerDict.TryAdd(PacketId.S_BroadcastEnterGame, (packet, session) => S_BroadcastEnterGameHandle(packet, session));
		_handlerDict.TryAdd(PacketId.S_BroadcastStartGame, (packet, session) => S_BroadcastStartGameHandle(packet, session));
		_handlerDict.TryAdd(PacketId.S_GameFrameInfo, (packet, session) => S_GameFrameInfoHandle(packet, session));
		_handlerDict.TryAdd(PacketId.S_BroadcastStartNewRound, (packet, session) => S_BroadcastStartNewRoundHandle(packet, session));
		_handlerDict.TryAdd(PacketId.S_BroadcastEndGame, (packet, session) => S_BroadcastEndGameHandle(packet, session));
	}

	public static void HandlePacket(BasePacket packet, ServerSession session)
	{
		if (packet == null) return;
		if (_handlerDict.TryGetValue((PacketId)packet.Id, out Action<BasePacket, ServerSession> action) == false)
			throw new Exception();
		action.Invoke(packet, session);
	}

	private static void S_InitHandle(BasePacket packet, ServerSession session)
	{
		System.Diagnostics.Debug.Assert(packet is S_Init);
		JobMgr.PushUnityJob(() => Scene.MoveTo(SceneType.Login, null));
	}

	private static void S_LoginHandle(BasePacket packet, ServerSession session)
	{
		var req = packet as S_Login;
		if (Scene.CurrentScene is not Scene_Loading loadingScene || loadingScene.IsReady == false)
		{
			return;
		}

		if (req.result == false)
		{
			JobMgr.PushUnityJob(Application.Quit);
		}

		User.Init(req);

		JobMgr.PushUnityJob(loadingScene.LoginSuccess);
	}

	private static void S_EnterLobbyHandle(BasePacket packet, ServerSession session)
	{
		System.Diagnostics.Debug.Assert(packet is S_EnterLobby);
		JobMgr.PushUnityJob(() => Scene.MoveTo(SceneType.Lobby, NetObjectType.Character_Shelly));
	}

	private static void S_EnterGameHandle(BasePacket packet, ServerSession session)
	{
		var req = packet as S_EnterGame;
		if (req.TeamId == -1) return;

		User.TeamId = req.TeamId;
		User.CharType = (NetObjectType)req.PlayerInfo.CharacterType;
	}

	private static void S_GameFrameInfoHandle(BasePacket packet, ServerSession session)
	{
		var req = packet as S_GameFrameInfo;
		var game = Scene.CurrentScene as Scene_Map1;
		if (game is null || game.IsReady == false)
		{
			UnityEngine.Debug.LogError("scene is not game or scene is not ready");
			return;
		}

		game.HandleGameState(req);
	}

	private static void S_BroadcastEnterGameHandle(BasePacket packet, ServerSession session)
	{
		var req = packet as S_BroadcastEnterGame;
		if (Scene.CurrentScene is not Scene_Map1 game || game.IsReady == false) return;
		JobMgr.PushUnityJob(() => game.Enter(req.TeamId, (NetObjectType)req.Charactertype));
	}

	private static void S_BroadcastStartGameHandle(BasePacket packet, ServerSession session)
	{
		var req = packet as S_BroadcastStartGame;

		JobMgr.PushUnityJob(() =>
		{
			if (Scene.CurrentScene is not Scene_Map1 game || game.IsReady == false) return;
			var length = req.CharacterTypeArr.Length;
			for (int i = 0; i < length; i++)
			{
				game.Enter((short)i, (NetObjectType)req.CharacterTypeArr[i]);
			}

			game.StartGame(req.WaitTime);
		});
	}

	private static int _retryCount;
	private static void S_BroadcastSearchPlayerHandle(BasePacket packet, ServerSession session)
	{
		var req = packet as S_BroadcastSearchPlayer;
		if (Scene.CurrentScene is not Scene_SearchingPlayers scene || scene.IsReady == false)
		{
			if (_retryCount > 10)
			{
				Loggers.Error.Error("handling S_BroadcastSearchPlayerHandle failed more than 10 time");
				return;
			}

			Loggers.Error.Error("handling S_BroadcastSearchPlayerHandle failed {0}", _retryCount++);
			JobMgr.PushUnityJob(() => Timing.CallDelayed(1, () => S_BroadcastSearchPlayerHandle(packet, session)));
			return;
		}

		_retryCount = 0;
		JobMgr.PushUnityJob(() => scene.UpdatePlayerFound(req.FoundPlayersCount));
	}

	private static void S_GameReadyHandle(BasePacket packet, ServerSession session)
	{
		var req = packet as S_GameReady;
		if (Scene.CurrentScene is not Scene_SearchingPlayers scene || scene.IsReady == false)
		{
			Loggers.Error.Error("Scene_SearchingPlayers is not ready yet");
			JobMgr.PushUnityJob(() => S_GameReadyHandle(packet, session));
			return;
		}

		JobMgr.PushUnityJob(scene.OnGameReady);
	}

	private static void S_SyncTimeHandle(BasePacket packet, ServerSession session)
	{
		var req = packet as S_SyncTime;
		if (Network.RTT == 0)
		{
			Network.RTT = (DateTime.UtcNow - DateTime.FromFileTimeUtc(req.ClientLocalTime)).Milliseconds;
		}
		else
		{
			Network.RTT = (Network.RTT + (DateTime.UtcNow - DateTime.FromFileTimeUtc(req.ClientLocalTime)).Milliseconds) / 2;
		}

		JobMgr.PushUnityJob(() => UnityEngine.Debug.Log($"Latency : {Network.Latency}"));
	}

	private static void S_BroadcastStartNewRoundHandle(BasePacket packet, ServerSession session)
	{
		var req = packet as S_BroadcastStartNewRound;
		if (Scene.CurrentScene is not Scene_Map1 scene || scene.IsReady == false)
		{
			Loggers.Error.Error("Scene_Map1 is not ready yet");
			return;
		}

		//JobMgr.PushUnityJob(() => Timing.CallDelayed((req.WaitMilliseconds - Network.Latency) / 1000f, scene.StartNewRound));
	}

	private static void S_BroadcastEndGameHandle(BasePacket packet, ServerSession session)
	{
		var req = packet as S_BroadcastEndGame;
		if (Scene.CurrentScene is not Scene_Map1 scene || scene.IsReady == false)
		{
			Loggers.Error.Error("Scene_Map1 is not ready yet");
			return;
		}

		JobMgr.PushUnityJob(scene.EndGame);
	}
}
