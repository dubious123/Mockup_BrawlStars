using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using MEC;

using Unity.VectorGraphics;

using UnityEngine;

[ExecuteInEditMode]
public class CenterScores : MonoBehaviour
{
	[SerializeField] private SVGImage[] _scores;
	[SerializeField] private Sprite _imageScoreWhite;
	[SerializeField] private Sprite _imageScoreGray;
	[SerializeField] private Sprite _imageScoreBlue;
	[SerializeField] private Sprite _imageScoreRed;
	[SerializeField] private float _scoreBigScale;

	[Header("Debug")]
	[SerializeField] private int _currentScore;

	private SVGImage _currentScoreUI;

	private void Start()
	{
		Reset();
	}

	public void OnRoundStart()
	{
		_currentScoreUI = _scores[_currentScore];
		_currentScoreUI.sprite = _imageScoreWhite;
		_currentScoreUI.rectTransform.localScale = new Vector3(_scoreBigScale, _scoreBigScale, _scoreBigScale);
	}

	public void OnPlayerWin()
	{
		_scores[_currentScore].sprite = _imageScoreBlue;
		InternalHandleRoundEnd();
	}

	public void OnPlayerLose()
	{
		_scores[_currentScore].sprite = _imageScoreRed;
		InternalHandleRoundEnd();
	}

	public void OnPlayerDraw()
	{
		_scores[_currentScore].sprite = _imageScoreGray;
		InternalHandleRoundEnd();
	}

	public void Reset()
	{
		_currentScore = 0;
		foreach (var score in _scores)
		{
			score.sprite = _imageScoreGray;
			score.rectTransform.localScale = Vector3.one;
		}
	}

	private void InternalHandleRoundEnd()
	{
		_currentScoreUI = _scores[_currentScore++];
		Timing.RunCoroutine(CoShrink());
	}

	private void MoveNext()
	{
		++_currentScore;
	}

	private IEnumerator<float> CoShrink()
	{
		var targetScale = Vector3.one;
		var startScale = new Vector3(_scoreBigScale, _scoreBigScale, _scoreBigScale);
		for (float delta = 0; delta < 1; delta += Time.deltaTime * 4)
		{
			_currentScoreUI.rectTransform.localScale = Vector3.Lerp(startScale, targetScale, (delta) * (2 - delta));
			yield return 0;
		}

		_currentScoreUI.rectTransform.localScale = targetScale;
		yield break;
	}
}
