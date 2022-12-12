using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using MEC;

using UnityEngine;
using UnityEngine.UI;

public class Map00UI : MonoBehaviour
{
	[SerializeField] private GameObject _uiTop;
	[SerializeField] private Image[] _profileIcons;
	[SerializeField] private UIWelcomeAnim _welcomeAnim;

	#region for debug
	[SerializeField] private GameCamWelcomeMove _camAnim;
	#endregion

	private void Start()
	{
		Reset();
	}

	public void Reset()
	{
		_uiTop.SetActive(false);
		_welcomeAnim.Reset();
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

			_profileIcons[cp.TeamId].sprite = cp.ProfileIcon;
		}

		_welcomeAnim.PlayAnim(() =>
		{
			_uiTop.SetActive(true);
			onCompleted?.Invoke();
		});
		_camAnim.enabled = true;
	}
}
