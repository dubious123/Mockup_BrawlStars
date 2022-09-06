using MEC;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UITool_Fade : MonoBehaviour
{
	#region Serialize Fields
	[SerializeField] TMP_Text _targetText;
	[SerializeField] Color _startColor;
	[SerializeField] float _startTime;
	[SerializeField] float _endTime;
	#endregion
	float _fadeSpeed;
	private void Awake()
	{
		_targetText.alpha = 0;
		_fadeSpeed = _startColor.a / (_endTime - _startTime);
	}
	public void StartFade()
	{
		Timing.KillCoroutines("Fade");
		Timing.RunCoroutine(Co_Fade(), "Fade");
	}

	private IEnumerator<float> Co_Fade()
	{
		float _deltaTime = 0f;
		_targetText.color = _startColor;
		float _alpha = _startColor.a;
		while (_deltaTime < _startTime)
		{
			_deltaTime += Timing.DeltaTime;
			yield return Timing.WaitForOneFrame;
		}
		while (_deltaTime < _endTime)
		{
			_deltaTime += Timing.DeltaTime;
			_targetText.alpha -= _fadeSpeed * Timing.DeltaTime;
			yield return Timing.WaitForOneFrame;
		}
		_targetText.alpha = 0;
		yield break;

	}
}
