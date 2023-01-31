using System.Collections.Generic;

using MEC;

using Unity.VectorGraphics;

using UnityEngine;

public class HudPowerCircle : MonoBehaviour
{
	[SerializeField] private SVGImage _powerCircleYellow;
	[SerializeField] private SVGImage _powerCircleBlue;
	[SerializeField] private float _spinSpeed;
	[SerializeField] private float _bigScale;
	private void Start()
	{
		GameInput.BasicAttackInputAction.started += _ => OnSelected();
	}

	private void Update()
	{
		transform.Rotate(new Vector3(0, _spinSpeed * Time.deltaTime, 0));
	}

	public void Reset()
	{
		_powerCircleBlue.enabled = false;
		_powerCircleYellow.enabled = false;
		transform.rotation = Quaternion.identity;
	}

	public void OnCharge()
	{
		_powerCircleBlue.enabled = true;
		_powerCircleYellow.enabled = false;
	}

	public void OnSelected()
	{
		_powerCircleBlue.enabled = false;
		_powerCircleYellow.enabled = true;
		Timing.RunCoroutine(Co_Shrink());
	}

	private IEnumerator<float> Co_Shrink()
	{
		var startScale = Vector3.one * _bigScale;
		for (var delta = 0f; delta < 0.5f; delta += Time.deltaTime)
		{
			transform.localScale = Vector3.Lerp(startScale, Vector3.one, delta * 2);
			yield return 0f;
		}

		yield break;
	}
}
