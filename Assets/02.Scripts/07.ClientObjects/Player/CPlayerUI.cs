using System.Collections;
using System.Collections.Generic;

using Server.Game;

using UnityEngine;

public class CPlayerUI : MonoBehaviour
{
	[SerializeField] private HudHP _hudHp;
	[SerializeField] private Sprite _selectCircleSelf;
	[SerializeField] private Sprite _selectCircleRed;
	[SerializeField] private Sprite _selectCircleBlue;
	[SerializeField] private SpriteRenderer _selectCircle;
	[SerializeField] private MoveCircle _moveCircle;

	public void Init(NetCharacter character, short teamId)
	{
		if (teamId == User.TeamId)
		{
			_selectCircle.sprite = _selectCircleSelf;
			_moveCircle.gameObject.SetActive(true);
		}
		else if (character.Team == User.Team)
		{
			_selectCircle.sprite = _selectCircleBlue;
		}
		else
		{
			_selectCircle.sprite = _selectCircleRed;
		}

		_hudHp.Init();
	}

	public void HandleOneFrame()
	{
		_hudHp.HandleOneFrame();
	}

	public void OnMatchStart()
	{

	}

	public void OnRoundStart()
	{
		gameObject.SetActive(true);
	}

	public void OnRoundClear()
	{
		gameObject.SetActive(false);
	}

	public void OnRoundReset()
	{
		_hudHp.OnRoundReset();
	}

	public void OnPlayerDead()
	{
		gameObject.SetActive(false);
	}
}
