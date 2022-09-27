using ServerCore;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using static Enums;
using static ServerCore.Utils.Enums;

public static class PacketHandler
{
	static ConcurrentDictionary<PacketId, Action<BasePacket, ServerSession>> _handlerDict;

	static PacketHandler()
	{
		_handlerDict = new ConcurrentDictionary<PacketId, Action<BasePacket, ServerSession>>();
		_handlerDict.TryAdd(PacketId.S_Init, (packet, session) => S_InitHandle(packet, session));
		_handlerDict.TryAdd(PacketId.S_Login, (packet, session) => S_LoginHandle(packet, session));
		_handlerDict.TryAdd(PacketId.S_EnterLobby, (packet, session) => S_EnterLobbyHandle(packet, session));
		_handlerDict.TryAdd(PacketId.S_EnterGame, (packet, session) => S_EnterGameHandle(packet, session));
		_handlerDict.TryAdd(PacketId.S_BroadcastEnterGame, (packet, session) => S_BroadcastEnterGameHandle(packet, session));
		_handlerDict.TryAdd(PacketId.S_BroadcastStartGame, (packet, session) => S_BroadcastStartGameHandle(packet, session));
		_handlerDict.TryAdd(PacketId.S_BroadcastGameState, (packet, session) => S_BroadcastGameStateHandle(packet, session));
		_handlerDict.TryAdd(PacketId.S_BroadcastMove, (packet, session) => S_BroadcastMoveHandle(packet, session));
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
		Debug.Assert(packet is S_Init);
		JobMgr.PushUnityJob(() => Scene.MoveTo(SceneType.Login, null));
	}

	private static void S_LoginHandle(BasePacket packet, ServerSession session)
	{
		var req = packet as S_Login;
		if (Scene.CurrentScene is not Scene_Login loginScene || loginScene.IsReady == false) return;
		if (req.result == false)
		{
			JobMgr.PushUnityJob(loginScene.OnLoginFailed);
			return;
		}
		JobMgr.PushUnityJob(() => loginScene.OnLoginSuccess(req));
	}

	private static void S_EnterLobbyHandle(BasePacket packet, ServerSession session)
	{
		Debug.Assert(packet is S_EnterLobby);
		JobMgr.PushUnityJob(() => Scene.MoveTo(SceneType.Lobby, CharacterType.Dog));
	}

	private static void S_EnterGameHandle(BasePacket packet, ServerSession session)
	{
		var req = packet as S_EnterGame;
		if (req.TeamId == -1 || req.PlayerInfoArr[req.TeamId].CharacterType == 0) return;
		User.TeamId = req.TeamId;
		JobMgr.PushUnityJob(() => Scene.MoveTo(SceneType.Game, req));
	}





	private static void S_BroadcastGameStateHandle(BasePacket packet, ServerSession session)
	{
		var req = packet as S_BroadcastGameState;
		var game = Scene.CurrentScene as Scene_Map1;
		if (game is null || game.IsReady == false)
		{
			UnityEngine.Debug.LogError("scene is not game or scene is not ready");
			return;
		}
		game.EnqueueFrameInfo(new GameFrameInfo()
		{
			LookInput = req.PlayerLookDirArr,
			MoveInput = req.PlayerMoveDirArr,
			StartTick = req.StartTick,
			TargetTick = req.TargetTick
		});

	}

	private static void S_BroadcastEnterGameHandle(BasePacket packet, ServerSession session)
	{
		var req = packet as S_BroadcastEnterGame;
		if (Scene.CurrentScene is not Scene_Map1 game || game.IsReady == false) return;
		JobMgr.PushUnityJob(() => game.Enter(req.TeamId, (CharacterType)req.Charactertype));

	}
	private static void S_BroadcastStartGameHandle(BasePacket packet, ServerSession session)
	{
		var req = packet as S_BroadcastStartGame;

		JobMgr.PushUnityJob(() =>
		{
			if (Scene.CurrentScene is not Scene_Map1 game || game.IsReady == false) return;
			game.StartGame(req.WaitTime);
		});
	}
	private static void S_BroadcastMoveHandle(BasePacket packet, ServerSession session)
	{
		var req = packet as S_BroadcastMove;
		if (Scene.CurrentScene is not Scene_Map1 game || game.IsReady == false) return;
		JobMgr.PushUnityJob(() => game.UpdatePlayer(req.TeamId, req.MoveDir, req.LookDir));
	}


}
