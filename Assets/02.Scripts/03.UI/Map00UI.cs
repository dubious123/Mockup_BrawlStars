using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using MEC;

using UnityEngine;

public class Map00UI : MonoBehaviour
{
	[SerializeField] private GameObject _uiTop;
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
		_welcomeAnim.PlayAnim(() =>
		{
			_uiTop.SetActive(true);
			onCompleted?.Invoke();
		});
		_camAnim.enabled = true;
	}
}
