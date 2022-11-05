﻿using System.Collections.Generic;

using Server.Game;

using TMPro;

using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CDogBash : MonoBehaviour, ICBaseSkill
{
	public bool Performing { get; set; }
	public bool Active { get; set; }
	public CPlayerDog Player { get; set; }
	public NetDogBash NetBash { get; set; }

	[SerializeField] private RawImage _skillIndicator;
	private TextMeshProUGUI _coolTimeIndicator;
	private IEnumerator<int> _coHandler;

	public void Init(CPlayerDog player)
	{
		Player = player;
		NetBash = (player.NPlayer as NetCharacterDog).Bash as NetDogBash;
		_coolTimeIndicator = GameObject.Find("Cool Time").GetComponent<TextMeshProUGUI>();
		_coolTimeIndicator.enabled = false;
	}

	public void HandleOneFrame()
	{
		if (NetBash.Holding)
		{
			_skillIndicator.enabled = true;
			_skillIndicator.rectTransform.sizeDelta = new Vector2(100, (float)NetBash.MaxBashDistance * ((float)NetBash.CurrentHoldFrame / NetBash.MaxHoldingFrame) * 100);
		}
		else
		{
			_skillIndicator.enabled = false;
		}

		Player.Animator.SetBool(AnimatorMeta.Dog_Bash, NetBash.Bashing);

		if (NetBash.CurrentCooltime > 0)
		{
			_coolTimeIndicator.enabled = true;
			_coolTimeIndicator.text = (NetBash.CurrentCooltime * Time.fixedDeltaTime).ToString("0.0");
		}
		else
		{
			_coolTimeIndicator.enabled = false;
		}
	}
}
