using Server.Game;

using Unity.VisualScripting;

using UnityEngine;

public class CProjectileSpikeNeedleGrenade : CProjectile
{
	[SerializeField] private AudioClip _explodeSound;
	[SerializeField] private Material _matBlue0;
	[SerializeField] private Material _matBlue1;
	[SerializeField] private Material _matRed0;
	[SerializeField] private Material _matRed1;
	[SerializeField] private Color _colorBlue0;
	[SerializeField] private Color _colorBlue1;
	[SerializeField] private Color _colorRed0;
	[SerializeField] private Color _colorRed1;
	[SerializeField] private float _spinSpeed;
	[SerializeField] private MeshRenderer[] _meshRenderers;

	private float _deltaTime;

	public override void Init(NetProjectile projectile)
	{
		base.Init(projectile);
		(var mat0, var mat1, var color0, var color1) = User.Team == projectile.Owner.GetComponent<NetCharacter>().Team ?
			(_matBlue0, _matBlue1, _colorBlue0, _colorBlue1) : (_matRed0, _matRed1, _colorRed0, _colorRed1);

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

		_meshRenderers[0].material = mat0;
		for (int i = _meshRenderers.Length - 1; 0 < i; --i)
		{
			_meshRenderers[i].material = mat1;
		}
	}

	protected override void DeActivate()
	{
		base.DeActivate();
		Audio.PlayOnce(_explodeSound, 0.5f);
	}

	private void LateUpdate()
	{
		_deltaTime += Time.deltaTime;
		var angle = -_spinSpeed * _deltaTime;
		BulletGo.transform.rotation = Quaternion.Euler(angle, angle, 0);
	}
}
