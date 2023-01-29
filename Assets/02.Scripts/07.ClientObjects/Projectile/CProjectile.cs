using UnityEngine;

public class CProjectile : ClientBaseComponent<NetProjectile>
{
	[SerializeField] private GameObject _bulletGo;
	[SerializeField] private ParticleSystem _onDisableEffect;
	[SerializeField] private TrailRenderer[] _trails;
	public bool IsAlive => _projectile.Active;

	protected new NProjectileSnapshot Now { get; set; }
	protected new NProjectileSnapshot Next { get; set; }

	private NetProjectile _projectile;

	public override void Init(NetProjectile projectile)
	{
		_projectile = projectile;
		Now = new NProjectileSnapshot();
		Now.TakePicture(projectile);
		Next = new NProjectileSnapshot();
		Next.TakePicture(projectile);
		transform.SetPositionAndRotation((Vector3)_projectile.Position, (Quaternion)_projectile.Rotation);
		Active = false;
	}

	public override void OnNetFrameUpdate()
	{
		(Now, Next) = (Next, Now);
		Next.TakePicture(_projectile);
		if (Now.Active is true && Active is false)
		{
			Activate();
		}
		else if (Now.Active is false && Active is true)
		{
			DeActivate();
		}
	}

	public override void Interpretate(float ratio)
	{
		var pos = Vector3.Lerp(Now.Position, Next.Position, ratio);
		transform.position = new Vector3(pos.x, 1, pos.z);
	}

	public void Clear()
	{
		if (Active is true)
		{
			DeActivate();
		}
	}

	protected void Activate()
	{
		transform.position = Now.Position;
		foreach (var trail in _trails)
		{
			trail.Clear();
		}
		_bulletGo.SetActive(true);
		_onDisableEffect.gameObject.SetActive(false);
		Active = true;
	}

	protected void DeActivate()
	{
		_bulletGo.SetActive(false);
		_onDisableEffect.gameObject.SetActive(true);
		_onDisableEffect.Play(true);
		Active = false;
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
