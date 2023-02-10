using System;

using Server.Game.GameRule;

using UnityEngine;

public class Map00UI : MonoBehaviour
{
	[SerializeField] private GameObject _uiTop;
	[SerializeField] private ProfileHolder[] _profileHoldersBlue;
	[SerializeField] private ProfileHolder[] _profileHoldersRed;
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
		//PlayWelcomeAnim((Scene.CurrentScene as Scene_Map1).StartGame);
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

	public void PlayWelcomeAnim(Action onCompleted = null)
	{
		var scene = Scene.CurrentScene as Scene_Map1;
		foreach (var cp in scene.ClientGameLoop.PlayerSystem)
		{
			if (cp is null)
			{
				continue;
			}

			GetHolder(cp).ProfileImage.sprite = cp.ProfileIcon;
		}

		_welcomeAnim.PlayAnim(() =>
		{
			_uiTop.SetActive(true);
			onCompleted?.Invoke();
		});

		_camAnim.enabled = true;
	}

	public void OnRoundStart()
	{
		_centerScores.OnRoundStart();
		_clock.OnRoundStart();
	}

	public void OnRoundEnd(GameRule00.RoundResult result)
	{
		if (result == GameRule00.RoundResult.Blue && User.Team == Enums.TeamType.Blue ||
			result == GameRule00.RoundResult.Red && User.Team == Enums.TeamType.Red)
		{
			_centerScores.OnPlayerWin();
			_gameMessage.ChangeText("Knockout win");
		}
		else if (result == GameRule00.RoundResult.Blue && User.Team == Enums.TeamType.Red ||
			result == GameRule00.RoundResult.Red && User.Team == Enums.TeamType.Blue)
		{
			_centerScores.OnPlayerLose();
			_gameMessage.ChangeText("Knockout lose");
		}
		else
		{
			_centerScores.OnPlayerDraw();
			_gameMessage.ChangeText("Knockout draw");
		}
	}

	public void OnRoundReset()
	{
		foreach (var profile in _profileHoldersBlue)
		{
			profile.Reset();
		}
		foreach (var profile in _profileHoldersRed)
		{
			profile.Reset();
		}

		_gameMessage.OnRoundReset();
		_clock.Reset();
	}

	public void OnMatchOver()
	{
		_gameMessage.ChangeText("Match over");
	}

	public void OnPlayerDead(ClientCharacter player)
	{
		GetHolder(player).OnDead();
	}

	private ProfileHolder GetHolder(ClientCharacter player)
	{
		return player.Team == User.Team ? _profileHoldersBlue[player.TeamId / 2] :
										  _profileHoldersRed[player.TeamId / 2];
	}
}
