using System;
using System.Collections;
using System.Collections.Generic;

using MEC;

using Unity.Mathematics;

using UnityEngine;

public class UIClock : MonoBehaviour
{
	[SerializeField] private TextWithShadow _text;
	[SerializeField] private Color _red;
	[SerializeField] private Color _yellow;
	private Scene_Map1 _scene;
	private CoroutineHandle _coShrinkHandle;
	private int _min;
	private int _second;

	public UIGameMessage _message;

	public void OnRoundStart()
	{
		_scene = _scene != null ? _scene : Scene.CurrentScene as Scene_Map1;
		UpdateClock();
		_text.BodyColor = Color.white;
	}

	public void UpdateClock()
	{
		var _remainedSecond = (_scene.MaxTick - _scene.CurrentTick) / 60;
		var min = Math.DivRem(_remainedSecond, 60, out var second);
		if (min == _min && second == _second)
		{
			return;
		}

		if (min == 0 && second < 16)
		{
			Timing.KillCoroutines(_coShrinkHandle);
			var textColor = second <= 10 ? _red : _yellow;
			_coShrinkHandle = Timing.RunCoroutine(CoBounce(textColor));
			_message.ChangeText(second.ToString());
		}

		_text.Text = string.Format("{0}:{1:00}", min, second);
		_min = min;
		_second = second;
	}

	private IEnumerator<float> CoBounce(Color targetColor)
	{
		var targetScale = Vector3.one;
		var startScale = new Vector3(1.5f, 1.5f, 1.5f);
		for (float delta = 0; delta < 1; delta += Time.deltaTime)
		{
			transform.localScale = Vector3.Lerp(startScale, targetScale, (delta) * (2 - delta));
			_text.BodyColor = Color.Lerp(Color.white, targetColor, 4 * (delta) * (2 - delta));
			yield return 0;
		}

		transform.localScale = targetScale;
		yield break;
	}
}
