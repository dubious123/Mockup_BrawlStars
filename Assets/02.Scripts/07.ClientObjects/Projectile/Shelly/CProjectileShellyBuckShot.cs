using Server.Game;

using UnityEngine;

public class CProjectileShellyBuckShot : CProjectile
{
	[SerializeField] private Sprite _spriteBlue;
	[SerializeField] private Sprite _spriteRed;
	[SerializeField] private Color _trailBlue;
	[SerializeField] private Color _trailRed;

	public override void Init(NetProjectile projectile)
	{
		base.Init(projectile);
		var team = NProjectile.Owner.GetComponent<NetCharacter>().Team;
		(var sprite, var color) = User.Team == team ? (_spriteBlue, _trailBlue) : (_spriteRed, _trailRed);

		foreach (var renderer in Renderers)
		{
			renderer.sprite = sprite;
		}
	}
}
