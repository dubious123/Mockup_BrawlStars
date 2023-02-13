using Server.Game;

using UnityEngine;

public class CProjectileShellySuperShell : CProjectile
{
	[SerializeField] private Sprite _spriteBlue;
	[SerializeField] private Sprite _spriteRed;
	[SerializeField] private Color _trailBlue;
	[SerializeField] private Color _trailRed;
	[SerializeField] private float _spinSpeed;

	private float _deltaTime;

	public override void Init(NetProjectile projectile)
	{
		base.Init(projectile);
		_deltaTime = 0;
		var team = NProjectile.Owner.GetComponent<NetCharacter>().Team;
		(var sprite, var color) = User.Team == team ? (_spriteBlue, _trailBlue) : (_spriteRed, _trailRed);

		foreach (var renderer in Renderers)
		{
			renderer.sprite = sprite;
		}

		foreach (var trail in Trails)
		{
			trail.startColor = color;
			trail.endColor = new Color(color.r, color.g, color.b, 0f);
		}
	}

	private void LateUpdate()
	{
		_deltaTime += Time.deltaTime;
		foreach (var renderer in Renderers)
		{
			renderer.transform.rotation = Quaternion.Euler(90, 0, -_spinSpeed * _deltaTime);
		}
	}
}
