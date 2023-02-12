using Server.Game;

using Unity.VisualScripting;

using UnityEngine;

public class CProjectileSpikeNeedleGrenade : CProjectile
{
	[SerializeField] private Color _colorBlue0;
	[SerializeField] private Color _colorBlue1;
	[SerializeField] private Color _colorRed0;
	[SerializeField] private Color _colorRed1;
	[SerializeField] private float _spinSpeed;

	private float _deltaTime;

	public override void Init(NetProjectile projectile)
	{
		base.Init(projectile);
		(var color0, var color1) = User.Team == projectile.Owner.GetComponent<NetCharacter>().Team ?
			(_colorBlue0, _colorBlue1) : (_colorRed0, _colorRed1);

		var main = OnDisableEffect.main;
		main.startColor = color0;
		var grad = OnDisableEffect.colorOverLifetime.color.gradient;

		for (int i = 0; i < grad.colorKeys.Length; ++i)
		{
			if (grad.colorKeys[i].time == 0)
			{
				grad.colorKeys[i].color = color1;
				break;
			}
		}
	}

	private void LateUpdate()
	{
		_deltaTime += Time.deltaTime;
		var angle = -_spinSpeed * _deltaTime;
		BulletGo.transform.rotation = Quaternion.Euler(angle, angle, 0);
	}
}
