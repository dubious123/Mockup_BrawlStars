using Server.Game;

using UnityEngine;

public class CProjectileShellyBuckShot : CProjectile
{
	[SerializeField] private Sprite _spriteRed;
	[SerializeField] private Sprite _spriteBlue;
	[SerializeField] private Color _trailBlue;
	[SerializeField] private Color _trailRed;
	[SerializeField] private SpriteRenderer[] _bulletRenderers;

	public override void Init(NetProjectile projectile)
	{
		base.Init(projectile);
		var team = NProjectile.Owner.GetComponent<NetCharacter>().Team;
		var sprite = User.Team == team ? _spriteBlue : _spriteRed;
		var color = User.Team == team ? _trailBlue : _trailRed;

		foreach (var renderer in _bulletRenderers)
		{
			renderer.sprite = sprite;
		}

		foreach (var trail in Trails)
		{
			trail.startColor = color;
		}
	}
}
