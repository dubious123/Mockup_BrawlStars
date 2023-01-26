using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

using MEC;

using Server.Game;
using Server.Game.Data;
using Server.Game.GameRule;

using UnityEngine.InputSystem;

using static Enums;


public class NetGameLoopHandler
{
	public ref int GameLoopLock => ref _gameLoopLock;
	public int CurrentFrameCount => _world.GameRule.CurrentRoundFrameCount;
	public NetWorld World => _world;
	public GameState State { get; private set; } = GameState.Waiting;

	private int _gameLoopLock;
	private NetWorld _world;
	private ConcurrentQueue<GameFrameInfo> _frameInfoQueue = new();
	private System.Timers.Timer _gameLoopTimer;
	private bool _gameEnded = false;
	private double _interval = 1000d / 60;

	public void Init(WorldData data, GameStartInfo startInfo)
	{
		_world = new(data, new GameRule00()
		{
			OnRoundEnd = OnRoundEnd,
			OnMatchOver = OnMatchOver,
		});

		for (int i = 0; i < startInfo.CharacterType.Length; i++)
		{
			_world.ObjectBuilder.GetNewObject(startInfo.CharacterType[i]);
		}

		_world.Reset();
		_gameLoopTimer = new System.Timers.Timer(_interval);
		_gameLoopTimer.Elapsed += MoveGameLoop;
		_gameLoopTimer.AutoReset = false;
	}

	public void StartGame()
	{
		Loggers.Game.Information("---------------StartGame----------------\nMatch Start");
		GameInput.SendInput(-Config.FRAME_BUFFER_COUNT);
	}

	public void HandleGameInput(S_GameFrameInfo req)
	{
		var info = new GameFrameInfo(req);
		_frameInfoQueue.Enqueue(info);
		if (info.FrameNum > 0)
		{
			return;
		}
		else if (info.FrameNum < 0)
		{
			GameInput.SendInput(info.FrameNum + 1);
		}
		else
		{
			HandleRoundStart((DateTime.UtcNow - DateTime.FromFileTimeUtc(req.ServerSendTime)).Milliseconds - Network.RTT + 1000);
		}
	}

	private void HandleRoundStart(int waitMilliseconds)
	{
		Loggers.Debug.Information("Round Start Wait Time : {0}", waitMilliseconds);
		Task.Delay(waitMilliseconds).ContinueWith(_ =>
		{
			Loggers.Game.Information("Round Start");
			State = GameState.Started;
			_gameLoopTimer.Start();
		});
	}

	private void MoveGameLoop(object abj, ElapsedEventArgs args)
	{
		if (Interlocked.CompareExchange(ref _gameLoopLock, 1, 0) == 1)
		{
			return;
		}

		switch (State)
		{
			case GameState.Waiting:
				break;
			case GameState.Started:
				GameInput.SendInput(CurrentFrameCount + Config.FRAME_BUFFER_COUNT);
				HandleOneFrame();
				break;
			case GameState.Ended:
				Loggers.Debug.Information("Start Fake Update");
				_world.UpdateInputs(GameFrameInfo.GetDefault(Config.MAX_PLAYER_COUNT));
				_world.Update();
				break;
		}

		Interlocked.Exchange(ref _gameLoopLock, 0);
		if (_gameEnded is false)
		{
			_gameLoopTimer.Start();
		}
		else
		{
			_gameLoopTimer.Dispose();
		}
	}

	private void HandleOneFrame()
	{
		Loggers.Game.Information("---------------Frame [{0}]----------------", CurrentFrameCount);
		Loggers.Game.Information("Reserve Count : {0}", _frameInfoQueue.Count);
		if (_frameInfoQueue.TryDequeue(out var info) is false || CurrentFrameCount != info.FrameNum)
		{
			Loggers.Error.Information("frame queue is empty or invalid frameCount {0}", CurrentFrameCount);
			return;
		}

		_world.UpdateInputs(info);
		_world.Update();
		foreach (var player in _world.CharacterSystem.ComponentDict)
		{
			Loggers.Game.Information("Player [{0}]", player.NetObj.ObjectId.InstanceId);
			Loggers.Game.Information("Position [{0:x},{1:x},{2:x}]] : ", player.Position.x.RawValue, player.Position.y.RawValue, player.Position.z.RawValue);
		}

		Loggers.Game.Information("------------------------------------------");
	}

	private void OnRoundEnd(GameRule00.RoundResult result)
	{
		Loggers.Game.Information("Round End {0}", Enum.GetName(typeof(GameRule00.RoundResult), result));
		State = GameState.Ended;
		Task.Delay(Config.ROUND_END_WAIT_FRAMECOUNT).ContinueWith(t => HandleRoundClear());
	}

	private void HandleRoundClear()
	{
		Loggers.Game.Information("Round Clear");
		State = GameState.Waiting;
		Task.Delay(Config.ROUND_CLEAR_WAIT_FRAMECOUNT).ContinueWith(t => HandleRoundReset());
	}

	private void HandleRoundReset()
	{
		Loggers.Game.Information("Round Reset");
		_world.Reset();
		Task.Delay(Config.ROUND_RESET_WAIT_FRAMECOUNT).ContinueWith(t =>
		{
			_frameInfoQueue.Clear();
			GameInput.SendInput(-Config.FRAME_BUFFER_COUNT);
		});
	}

	private void OnMatchOver(GameRule00.MatchResult result)
	{
		Loggers.Game.Information("Match Over {0}", Enum.GetName(typeof(GameRule00.MatchResult), result));
		State = GameState.Ended;
		Task.Delay(Config.ROUND_END_WAIT_FRAMECOUNT).ContinueWith(t => _gameEnded = true);
	}
}

