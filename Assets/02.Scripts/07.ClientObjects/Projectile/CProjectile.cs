using UnityEngine;

public class CProjectile : ClientBaseComponent<NetProjectile>
{
	[SerializeField] private GameObject _bulletGo;
	[SerializeField] private ParticleSystem _onDisableEffect;
	[SerializeField] private SpriteRenderer[] _renderers;
	[field: SerializeField] protected TrailRenderer[] Trails { get; set; }

	public static CProjectileSystem System;
	public bool IsAlive => _projectile.Active;
	public NetProjectile NProjectile => _projectile;

	protected GameObject BulletGo => _bulletGo;
	protected ParticleSystem OnDisableEffect => _onDisableEffect;
	protected SpriteRenderer[] Renderers => _renderers;
	protected NProjectileSnapshot Now { get; set; }
	protected NProjectileSnapshot Next { get; set; }

	private NetProjectile _projectile;

	public override void Init(NetProjectile projectile)
	{
		_projectile = projectile;
		_bulletGo.SetActive(false);
		Now = new NProjectileSnapshot();
		Now.TakePicture(projectile);
		Next = new NProjectileSnapshot();
		Next.TakePicture(projectile);
		transform.SetPositionAndRotation((Vector3)_projectile.Position, (Quaternion)_projectile.Rotation);
		Activate();
	}

	public override void OnNetFrameUpdate()
	{
		(Now, Next) = (Next, Now);
		Next.TakePicture(_projectile);
	}

	public override void Interpretate(float ratio)
	{
		if (Now.Active is false)
		{
			DeActivate();
			return;
		}

		var pos = Vector3.Lerp(Now.Position, Next.Position, ratio);
		transform.position = new Vector3(pos.x, 1, pos.z);
	}

	public void Clear()
	{
		DeActivate();
	}

	protected virtual void Activate()
	{
		transform.position = Now.Position;
		foreach (var trail in Trails)
		{
			if (trail != null)
			{
				trail.Clear();
			}
		}

		_bulletGo.SetActive(true);
		_onDisableEffect.gameObject.SetActive(false);
		Active = true;
	}

	protected virtual void DeActivate()
	{
		_bulletGo.SetActive(false);
		_onDisableEffect.gameObject.SetActive(true);
		_onDisableEffect.Play(true);
		Active = false;
		System.External_Return(this);
	}

	protected class NProjectileSnapshot : SnapShot
	{
		public Vector3 Position;
		public Quaternion Rotation;
		public bool Active;

		public override void TakePicture(NetProjectile netComponent)
		{
			Position = (Vector3)netComponent.Position;
			Rotation = (Quaternion)netComponent.Rotation;
			Active = netComponent.Active;
		}
	}
}
