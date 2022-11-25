using System.Collections;
using System.Collections.Generic;

using MEC;

using Unity.Mathematics;
using Unity.VisualScripting;

using UnityEditor;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ProfileHolder : MonoBehaviour
{
	[SerializeField] private Color _deadColor;
	[SerializeField] private Image _profileImage;
	[SerializeField] private RectTransform _deadBar;
	[SerializeField] private Image _effect;
	[SerializeField] private float _effectTargetScale;


	private void Start()
	{
		Reset();
	}

	public void OnDead()
	{
		_profileImage.color = _deadColor;
		Timing.RunCoroutine(CoPlayDeadAnim());
	}

	public void Reset()
	{
		_profileImage.color = Color.white;
		_deadBar.gameObject.SetActive(false);
		_effect.gameObject.SetActive(false);
	}

	private IEnumerator<float> CoPlayDeadAnim()
	{
		var coBoom = PlayBoomEffect();
		var coBar = PlayDeadBar();
		for (float delta = 0; delta < 0.75f; delta += Time.deltaTime)
		{
			coBoom.MoveNext();
			yield return 0;
		}

		for (float delta = 0; delta < 1; delta += Time.deltaTime)
		{
			coBoom.MoveNext();
			coBar.MoveNext();
			yield return 0;
		}

		Debug.Log("Dead");
		yield break;
	}

	private IEnumerator<int> PlayBoomEffect()
	{
		_effect.gameObject.SetActive(true);
		var startScale = Vector3.one;
		var targetScale = startScale * _effectTargetScale;
		for (float delta = 0; delta < 1; delta += Time.deltaTime)
		{
			_effect.rectTransform.localScale = Vector3.Lerp(startScale, targetScale, delta);
			_effect.ChangeAlpha((1 - math.pow(delta, 4)));
			yield return 0;
		}
		_effect.rectTransform.localScale = startScale;
		_effect.gameObject.SetActive(false);
	}

	private IEnumerator<int> PlayDeadBar()
	{
		_deadBar.gameObject.SetActive(true);
		var targetScale = _deadBar.localScale;
		var startScale = new Vector3(1f, 0f, 1f);
		for (float delta = 0; delta < 1; delta += Time.deltaTime)
		{
			_deadBar.localScale = Vector3.Lerp(startScale, targetScale, 1 - math.pow(1 - delta, 4));
			yield return 0;
		}

		_deadBar.localScale = targetScale;
	}
}
