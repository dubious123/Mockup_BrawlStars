using Server.Game;

using UnityEngine;

public class CProjectileSpikeStickAroundGrenade : CProjectile
{
	[SerializeField] private AudioClip _explodeSound;
	[SerializeField] private Material _matBlue0;
	[SerializeField] private Material _matBlue1;
	[SerializeField] private Material _matBlue2;
	[SerializeField] private Material _matRed0;
	[SerializeField] private Material _matRed1;
	[SerializeField] private Material _matRed2;
	[SerializeField] private Color _colorBlue0;
	[SerializeField] private Color _colorBlue1;
	[SerializeField] private Color _colorRed0;
	[SerializeField] private Color _colorRed1;
	[SerializeField] private MeshRenderer[] _meshRenderers;
	[SerializeField] private MeshRenderer[] _meshRenderersPetal;

	public override void Init(NetProjectile projectile)
	{
		base.Init(projectile);
		(var mat0, var mat1, var mat2, var color0, var color1) = User.Team == projectile.Owner.GetComponent<NetCharacter>().Team ?
			(_matBlue0, _matBlue1, _matBlue2, _colorBlue0, _colorBlue1) : (_matRed0, _matRed1, _matRed2, _colorRed0, _colorRed1);

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

		foreach (var renderer in _meshRenderersPetal)
		{
			renderer.material = mat2;
		}
	}

	public override void Interpretate(float ratio)
	{
		base.Interpretate(ratio);
		var x = NProjectile.CurrentTravelTime / (float)NProjectile.MaxTravelTime;
		var height = x * (1 - x) * (16f * NProjectile.MaxTravelTime / 60f) + 1;
		transform.position = new Vector3(transform.position.x, height, transform.position.z);
	}

	protected override void DeActivate()
	{
		base.DeActivate();
		Audio.PlayOnce(_explodeSound, 0.5f);
	}
}
