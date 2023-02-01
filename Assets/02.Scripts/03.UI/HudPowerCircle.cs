using System.Collections.Generic;

using MEC;

using Server.Game;

using Unity.VectorGraphics;

using UnityEngine;

public class HudPowerCircle : MonoBehaviour
{
	[SerializeField] private SVGImage _powerCircleYellow;
	[SerializeField] private SVGImage _powerCircleBlue;
	[SerializeField] private float _spinSpeed;
	[SerializeField] private float _bigScale;

	private NetSpecialAttack _netSpecialAttack;
	private Transform _base;
	private bool _pressed;
	private float _deltaTime = 0f;

	public void Init(NetSpecialAttack netSpecialAttack)
	{
		_netSpecialAttack = netSpecialAttack;
		_pressed = false;
	}

	private void LateUpdate()
	{
		_deltaTime += Time.deltaTime;
		transform.rotation = Quaternion.Euler(90, 0, -_spinSpeed * _deltaTime);
		if (_netSpecialAttack.Holding)
		{
			_powerCircleBlue.enabled = false;
			_powerCircleYellow.enabled = true;
			if (_pressed is false)
			{
				Timing.RunCoroutine(Co_Shrink());
			}

			_pressed = true;
		}
		else if (_netSpecialAttack.CanAttack())
		{
			_powerCircleBlue.enabled = true;
			_powerCircleYellow.enabled = false;
			_pressed = false;
		}
		else
		{
			_powerCircleBlue.enabled = false;
			_powerCircleYellow.enabled = false;
			_pressed = false;
		}
	}

	public void Reset()
	{
		_powerCircleBlue.enabled = false;
		_powerCircleYellow.enabled = false;
		_pressed = false;
		_deltaTime = 0f;
		transform.rotation = Quaternion.identity;
	}

	private IEnumerator<float> Co_Shrink()
	{
		var startScale = Vector3.one * _bigScale;
		for (var delta = 0f; delta < 0.25f; delta += Time.deltaTime)
		{
			transform.localScale = Vector3.Lerp(startScale, Vector3.one, delta * 4);
			yield return 0f;
		}

		yield break;
	}
}
