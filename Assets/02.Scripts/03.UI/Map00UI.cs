using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using MEC;

using UnityEngine;

public class Map00UI : MonoBehaviour
{
	[SerializeField] private GameObject _uiTop;
	[SerializeField] private UIWelcomeAnim _welcomeAnim;

	private void Start()
	{
		Reset();
	}

	public void Reset()
	{
		_uiTop.SetActive(false);
		_welcomeAnim.Reset();
	}

	public void OnGameStart()
	{
		_welcomeAnim.PlayAnim(() => _uiTop.SetActive(true));
	}
}
