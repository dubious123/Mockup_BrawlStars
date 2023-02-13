using System.Collections.Generic;

using MEC;

using Server.Game;

using Unity.VectorGraphics;

using UnityEngine;

public class HudPowerCircle : MonoBehaviour
{
	[SerializeField] private Sprite _powerCircleYellow;
	[SerializeField] private Sprite _powerCircleBlue;
	[SerializeField] private Sprite _powerCircleRed;
	[SerializeField] private float _spinSpeed;
	[SerializeField] private float _bigScale;

	private SVGImage _image;
	private Sprite _spriteNotHolding;
	private NetSpecialAttack _netSpecialAttack;
	private bool _pressed;
	private float _deltaTime = 0f;

	public void Init(NetSpecialAttack netSpecialAttack)
	{
		_netSpecialAttack = netSpecialAttack;
		_pressed = false;
		_spriteNotHolding = netSpecialAttack.Character.Team == User.Team ? _powerCircleBlue : _powerCircleRed;
		_image = transform.GetChild(0).GetComponent<SVGImage>();
	}

	private void LateUpdate()
	{
		_deltaTime += Time.deltaTime;
		transform.rotation = Quaternion.Euler(90, 0, -_spinSpeed * _deltaTime);
		if (_netSpecialAttack.Holding)
		{
			_image.sprite = _powerCircleYellow;
			if (_pressed is false)
			{
				Timing.RunCoroutine(Co_Shrink());
				Audio.PlayerPowerSelected();
			}

			_image.enabled = true;
			_pressed = true;
		}
		else if (_netSpecialAttack.CanAttack())
		{
			_image.enabled = true;
			_image.sprite = _spriteNotHolding;
			_pressed = false;
		}
		else
		{
			_image.enabled = false;
			_pressed = false;
		}
	}

	public void Reset()
	{
		_image.enabled = false;
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
