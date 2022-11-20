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
}
