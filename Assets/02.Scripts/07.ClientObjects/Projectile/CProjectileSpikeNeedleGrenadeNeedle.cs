using Server.Game;

using UnityEngine;

public class CProjectileSpikeNeedleGrenadeNeedle : CProjectile
{
	[SerializeField] private Sprite _blue;
	[SerializeField] private Sprite _red;

	public override void Init(NetProjectile projectile)
	{
		base.Init(projectile);
		var angle = -(float)sMathf.Atan2(projectile.MoveDir.z, projectile.MoveDir.x) * Mathf.Rad2Deg;
		transform.rotation *= Quaternion.Euler(0, angle, 0);
		var sprite = User.Team == projectile.Owner.GetComponent<NetCharacter>().Team ? _blue : _red;
		foreach (var renderer in Renderers)
		{
			renderer.sprite = sprite;
		}
	}
}
