using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

public class TextWithShadow : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI _body;
	[SerializeField] private TextMeshProUGUI _shadow;

	public string Text
	{
		get => _body.text;

		set
		{
			_body.text = value;
			_shadow.text = value;
		}
	}

	public float Alpha
	{
		get => _body.alpha;
		set
		{
			_body.alpha = value;
			_shadow.alpha = value;
		}
	}
}
