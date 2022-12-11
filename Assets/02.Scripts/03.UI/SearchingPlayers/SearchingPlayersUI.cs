using System.Collections;
using System.Collections.Generic;

using MEC;

using TMPro;

using UnityEngine;
using UnityEngine.UIElements;

public class SearchingPlayersUI : MonoBehaviour
{
	[SerializeField] private RectTransform _shrinkTarget;
	[SerializeField] private RectTransform _rotateTarget;
	[SerializeField] private TextMeshProUGUI _text;

	private int _targetPlayersCount;
	private int _foundPlayers;

	public void Init(int targetPlayersCount)
	{
		_targetPlayersCount = targetPlayersCount;
		Timing.RunCoroutine(RotateImage().CancelWith(gameObject));
		Timing.RunCoroutine(ShrinkImage().CancelWith(gameObject));
	}

	public void UpdateText(int foundPlayers)
	{
		_foundPlayers = foundPlayers;
		_text.text = string.Format("Players found {0}/{1}", _foundPlayers, _targetPlayersCount);
	}

	public void OnGameReady()
	{

	}

	private IEnumerator<float> RotateImage()
	{
		while (true)
		{
			_rotateTarget.transform.Rotate(0, 0, -180 * Time.deltaTime);
			yield return 0f;
		}
	}

	private IEnumerator<float> ShrinkImage()
	{
		var scale = _shrinkTarget.localScale;
		for (float delta = 0; delta < 0.5f; delta += Time.deltaTime)
		{
			_shrinkTarget.localScale = Vector3.Lerp(scale, Vector3.one, delta * 2);
			yield return 0f;
		}
	}
}
