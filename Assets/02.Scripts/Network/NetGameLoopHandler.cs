using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

using Server.Game;
using Server.Game.Data;
using Server.Game.GameRule;

using static Enums;
using static Server.Game.GameRule.GameRule00;

public class NetGameLoopHandler
{
	public ref int GameLoopLock => ref _gameLoopLock;
	public int FrameNum => _world.GameRule.FrameNum;
	public NetWorld World => _world;
	public GameState State { get; private set; } = GameState.Waiting;
	public double Interval => _interval;
	public RoundResult RoundRes { get; private set; }
	public bool NetGameStarted { get; private set; }

	private volatile int _gameLoopLock;
	private NetWorld _world;
	private ConcurrentQueue<GameFrameInfo> _frameInfoQueue = new();
	private System.Timers.Timer _gameLoopTimer;
	private double _interval = 1000d / 60;
	private double _normalInterval = 1000d / 60;//60fps
	private double _fastInterval = 800d / 60; //75fps
	private double _slowInterval = 1200d / 60;//50fps
	private DateTime _lastUpdateTime;

	public ref DateTime GetLastUpdateTime() => ref _lastUpdateTime;

	public void Init(WorldData data, GameStartInfo startInfo)
	{
		_world = new(data, new GameRule00()
		{
			OnRoundEnd = OnRoundEnd,
			OnMatchOver = OnMatchOver,
		});

		for (int i = 0; i < startInfo.CharacterType.Length; i++)
		{
			var nPlayer = _world.ObjectBuilder.GetNewObject(startInfo.CharacterType[i]).GetComponent<NetCharacter>();
			if (i == User.TeamId)
			{
				User.Team = nPlayer.Team;
			}
		}

		_gameLoopTimer = new System.Timers.Timer(_interval);
		_gameLoopTimer.Elapsed += MoveGameLoop;
		_gameLoopTimer.AutoReset = false;
		_world.Reset();
	}

	public void StartGame()
	{
		Loggers.Game.Information("---------------StartGame----------------");
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
			NetGameStarted = true;
		});
	}

	private void MoveGameLoop(object abj, ElapsedEventArgs args)
	{
		while (Interlocked.CompareExchange(ref _gameLoopLock, 1, 0) == 1) { }

		_lastUpdateTime = args.SignalTime;
		switch (State)
		{
			case GameState.Started:
				HandleOneFrame();
				break;
			case GameState.RoundEnded:
				Loggers.Debug.Information("Start Fake Update");
				_world.UpdateInputs(GameFrameInfo.GetDefault(Config.MAX_PLAYER_COUNT));
				_world.Update();
				break;
			case GameState.MatchOvered:
				_gameLoopTimer.Dispose();
				Interlocked.Exchange(ref _gameLoopLock, 0);
				return;
		}

		Interlocked.Exchange(ref _gameLoopLock, 0);
		var targetInterval = _frameInfoQueue.Count > Config.FRAME_BUFFER_COUNT + 1 ? _fastInterval :
			_frameInfoQueue.Count < Config.FRAME_BUFFER_COUNT - 1 ? _slowInterval :
			_normalInterval;
		var netLoopFinishTime = (DateTime.Now - _lastUpdateTime).TotalMilliseconds;
		_interval = Math.Max(targetInterval - netLoopFinishTime, double.Epsilon);
		Loggers.Game.Information("Interval : {0}", _interval);
		_gameLoopTimer.Interval = _interval;
		_gameLoopTimer.Start();
		Loggers.Debug.Information("gameLoop elapsed : {0}, targetInterval : {1}", netLoopFinishTime, targetInterval);
	}

	private void HandleOneFrame()
	{
		Loggers.Game.Information("---------------Frame [{0}]----------------", FrameNum);
		Loggers.Game.Information("Reserve Count : {0}", _frameInfoQueue.Count);
		if (_frameInfoQueue.TryDequeue(out var info) is false || FrameNum != info.FrameNum)
		{
			Loggers.Error.Information("frame queue is empty or invalid frameCount {0}", FrameNum);
			return;
		}

		GameInput.SendInput(FrameNum + Config.FRAME_BUFFER_COUNT + 1);
		_world.UpdateInputs(info);
		_world.Update();
		foreach (var player in _world.CharacterSystem.ComponentDict)
		{
			Loggers.Game.Information("Player [{0}]", player.TeamId);
			Loggers.Game.Information("Position [{0:x},{1:x},{2:x}]] : ", player.Position.x.RawValue, player.Position.y.RawValue, player.Position.z.RawValue);
		}

		Loggers.Game.Information("------------------------------------------");
	}

	private void OnRoundEnd(RoundResult result)
	{
		Loggers.Game.Information("Round End {0}", Enum.GetName(typeof(GameRule00.RoundResult), result));
		State = GameState.RoundEnded;
		RoundRes = result;
		Task.Delay(Config.ROUND_END_WAIT_DELAY).ContinueWith(t => HandleRoundClear());
	}

	private void HandleRoundClear()
	{
		Loggers.Game.Information("Round Clear");
		State = GameState.RoundCleared;
		Task.Delay(Config.ROUND_CLEAR_WAIT_DELAY).ContinueWith(t => HandleRoundReset());
	}

	private void HandleRoundReset()
	{
		Loggers.Game.Information("Round Reset");
		State = GameState.Waiting;
		_world.Reset();
		_gameLoopTimer.Interval = _normalInterval;
		_interval = _normalInterval;
		Task.Delay(Config.ROUND_RESET_WAIT_DELAY).ContinueWith(t =>
		{
			_frameInfoQueue.Clear();
			GameInput.SendInput(-Config.FRAME_BUFFER_COUNT);
		});
	}

	private void OnMatchOver(MatchResult result)
	{
		Loggers.Game.Information("Match Over {0}", Enum.GetName(typeof(GameRule00.MatchResult), result));
		State = GameState.RoundEnded;
		Task.Delay(Config.ROUND_END_WAIT_DELAY).ContinueWith(t => State = GameState.MatchOvered);
	}
}

