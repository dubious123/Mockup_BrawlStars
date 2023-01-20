using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using MEC;

using Server.Game.GameRule;

using UnityEngine;
using UnityEngine.UI;

public class Map00UI : MonoBehaviour
{
	[SerializeField] private GameObject _uiTop;
	[SerializeField] private ProfileHolder[] _profileHolders;
	[SerializeField] private UIWelcomeAnim _welcomeAnim;
	[SerializeField] private UIGameMessage _gameMessage;
	[SerializeField] private UIClock _clock;

	#region for debug
	[SerializeField] private GameCamWelcomeMove _camAnim;
	#endregion

	private CenterScores _centerScores;

	private void Start()
	{
		_centerScores = GetComponentInChildren<CenterScores>(true);
		Reset();
	}

	public void Reset()
	{
		_uiTop.SetActive(false);
		_welcomeAnim.Reset();
	}

	public void HandleOneFrame()
	{
		_clock.UpdateClock();
	}

	public void OnGameStart(Action onCompleted = null)
	{
		var scene = Scene.CurrentScene as Scene_Map1;
		foreach (var cp in scene.CPlayers)
		{
			if (cp is null)
			{
				continue;
			}

			_profileHolders[cp.TeamId].ProfileImage.sprite = cp.ProfileIcon;
		}

		_welcomeAnim.PlayAnim(() =>
		{
			_uiTop.SetActive(true);
			onCompleted?.Invoke();
		});

		_camAnim.enabled = true;
	}

	//public void OnMatchStart()
	//{

	//}

	public void OnRoundStart()
	{
		_centerScores.OnRoundStart();
		_clock.OnRoundStart();
	}

	public void OnRoundEnd(GameRule00.RoundResult result)
	{
		if (result == GameRule00.RoundResult.Blue)
		{
			_centerScores.OnBlueWin();
			if (User.Team == Enums.TeamType.Blue)
			{
				_gameMessage.ChangeText("Knockout win");
			}
			else if (User.Team == Enums.TeamType.Red)
			{
				_gameMessage.ChangeText("Knockout lose");
			}
		}
		else
		{
			_centerScores.OnRedWin();
			if (User.Team == Enums.TeamType.Blue)
			{
				_gameMessage.ChangeText("Knockout lose");
			}
			else if (User.Team == Enums.TeamType.Red)
			{
				_gameMessage.ChangeText("Knockout win");
			}
		}
	}

	public void OnRoundClear()
	{

	}

	public void OnRoundReset()
	{
		foreach (var profile in _profileHolders)
		{
			profile.Reset();
		}

		_gameMessage.OnRoundReset();
	}

	public void OnMatchOver()
	{
		_gameMessage.ChangeText("Match over");
	}

	public void OnPlayerDead(uint playerId)
	{
		_profileHolders[playerId].OnDead();
	}
}
