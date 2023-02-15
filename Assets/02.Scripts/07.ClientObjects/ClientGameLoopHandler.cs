using System;
using System.Threading;

using Server.Game;

using UnityEngine;

using static Enums;

public class ClientGameLoopHandler : MonoBehaviour
{
	[field: SerializeField] public CProjectileSystem ProjectileSystem { get; private set; }
	[field: SerializeField] public CPlayerSystem CharacterSystem { get; private set; }
	[field: SerializeField] public CEnvSystem EnvSystem { get; private set; }

	[SerializeField] private Map00UI _mapUI;
	private IClientComponentSystem[] _systems;
	private NetGameLoopHandler _netGameLoop;
	private NetWorld _netWorld => _netGameLoop.World;
	private GameState _state;
	private int _frameNum;

	public void Init(NetGameLoopHandler netGameLoop)
	{
		_systems = new IClientComponentSystem[] { CharacterSystem, EnvSystem, ProjectileSystem };
		_netGameLoop = netGameLoop;
		ProjectileSystem.Init(_netWorld.ProjectileSystem);
		CharacterSystem.Init(_netWorld.CharacterSystem);
		EnvSystem.Init(_netWorld.EnvSystem);
		_frameNum = -Config.FRAME_BUFFER_COUNT;
		_mapUI = (Scene.CurrentScene as Scene_Map1).UI;
	}

	private void Update()
	{
		if (_netGameLoop.NetGameStarted is false)
		{
			return;
		}

		while (Interlocked.CompareExchange(ref _netGameLoop.GameLoopLock, 1, 0) == 1) { }
		try
		{
			var lastUpdateTime = _netGameLoop.GetLastUpdateTime();
			var expectedNextUpdateTime = lastUpdateTime.AddMilliseconds(_netGameLoop.Interval);
			var nowTime = DateTime.Now;
			Debug.Assert(lastUpdateTime <= nowTime);
			var ratio = expectedNextUpdateTime == lastUpdateTime ? 0 : (nowTime - lastUpdateTime) / (expectedNextUpdateTime - lastUpdateTime);
			Loggers.Game.Information("Client Handling Loop FrameNum[{0}]", _netGameLoop.FrameNum);
			Loggers.Debug.Information("last update time {0} expected next update time : {1}, nowTime : {2}, ratio : {3}", lastUpdateTime, expectedNextUpdateTime, nowTime, ratio);
			Math.Clamp(ratio, 0, 1);

			if (_state != _netGameLoop.State)
			{
				switch (_netGameLoop.State)
				{
					case GameState.Started:
						HandleRoundStart();
						break;
					case GameState.RoundEnded:
						HandleRoundEnd();
						goto Return;
					case GameState.RoundCleared:
						HandleRoundClear();
						break;
					case GameState.Waiting:
						HandleRoundReset();
						break;
					case GameState.MatchOvered:
						HandleMatchOver();
						goto Return;
				}
			}

			if (_frameNum == _netGameLoop.FrameNum)
			{
				Loggers.Debug.Information("Interpretate only");
				foreach (var system in _systems)
				{
					system.Interpretate((float)ratio);
				}
			}
			else if (_frameNum < _netGameLoop.FrameNum)
			{
				Loggers.Debug.Information("NetFrameUpdate");
				_mapUI.HandleOneFrame();
				foreach (var system in _systems)
				{
					system.OnNetFrameUpdate();
				}
			}
			else
			{
				//Reset
			}
		Return:
			_state = _netGameLoop.State;
			_frameNum = _netGameLoop.FrameNum;
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
		finally
		{
			Interlocked.Exchange(ref _netGameLoop.GameLoopLock, 0);

		}
	}

	private void HandleRoundStart()
	{
		_mapUI.OnRoundStart();
		foreach (var system in _systems)
		{
			system.OnRoundStart();
		}
	}

	private void HandleRoundEnd()
	{
		_mapUI.OnRoundEnd(_netGameLoop.RoundRes);
		foreach (var system in _systems)
		{
			system.OnRoundEnd();
		}
	}

	private void HandleRoundClear()
	{
		foreach (var system in _systems)
		{
			system.Clear();
		}
	}

	private void HandleRoundReset()
	{
		_mapUI.OnRoundReset();
		foreach (var system in _systems)
		{
			system.Reset();
		}
	}

	private void HandleMatchOver()
	{
		_mapUI.OnMatchOver(_netGameLoop.MatchRes);
		GameInput.SetActive(false);
		(Scene.CurrentScene as Scene_Map1).EndGame();
	}
}
