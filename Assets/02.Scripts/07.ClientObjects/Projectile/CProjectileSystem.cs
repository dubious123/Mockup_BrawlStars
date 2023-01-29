using System.Collections.Generic;

using Server.Game;

public class CProjectileSystem : CBaseComponentSystem<NetProjectile>
{
	private List<CProjectile> _projectiles;

	public void AddAndInitProjectile(CProjectile cProjectile, NetProjectile nProjectile)
	{
		_projectiles ??= new(Config.MAX_PLAYER_COUNT * 15);
		_projectiles.Add(cProjectile);
		cProjectile.Init(nProjectile);
	}

	public override void Init(NetBaseComponentSystem<NetProjectile> netSystem)
	{
	}

	public override void OnNetFrameUpdate()
	{
		foreach (var cProjectile in _projectiles)
		{
			cProjectile.OnNetFrameUpdate();
			cProjectile.Interpretate(0);
		}
	}

	public override void Interpretate(float ratio)
	{
		foreach (var cProjectile in _projectiles)
		{
			cProjectile.Interpretate(ratio);
		}
	}

	public override void OnRoundStart()
	{
	}

	public override void OnRoundEnd()
	{
	}

	public override void Clear()
	{
		foreach (var cProjectile in _projectiles)
		{
			cProjectile.Clear();
		}
	}

	public override void Reset()
	{
	}
}
