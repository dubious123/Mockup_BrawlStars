using System.Collections;
using System.Collections.Generic;

using MEC;

using TMPro;

using UnityEngine;

using static Server.Game.GameRule.GameRule00;

public class UIGameMessage : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI _text;

	public void OnRoundReset()
	{
		gameObject.SetActive(false);
	}

	public void ChangeText(string text)
	{
		gameObject.SetActive(true);
		_text.text = text;
		Timing.RunCoroutine(CoShrink());
	}

	private IEnumerator<float> CoShrink()
	{
		for (float delta = 0; delta < 1; delta += Time.deltaTime * 2)
		{
			transform.localScale = Vector3.Lerp(new Vector3(1.5f, 1.5f, 1.5f), Vector3.one, delta);
			yield return 0;
		}

		transform.localScale = Vector3.one;
		yield break;
	}
}
