using System;
using System.Collections;
using System.Collections.Generic;

using MEC;

using UnityEngine;

public class CProjectile : MonoBehaviour
{
	[SerializeField] private GameObject _bulletGo;
	[SerializeField] private ParticleSystem _onDisableEffect;
	public bool IsAlive => _projectile.Active;
	private NetProjectile _projectile;

	public void Init(NetProjectile projectile)
	{
		_projectile = projectile;
		transform.SetPositionAndRotation((Vector3)_projectile.Position, (Quaternion)_projectile.Rotation);
	}

	private void OnEnable()
	{
		_onDisableEffect.gameObject.SetActive(false);
		_bulletGo.SetActive(true);
	}

	private void Update()
	{
		if (_projectile.Active is false)
		{
			enabled = false;
			_bulletGo.SetActive(false);
			return;
		}

		//transform.SetPositionAndRotation((Vector3)_projectile.Position, (Quaternion)_projectile.Rotation);
		transform.position = new Vector3((float)_projectile.Position.x, 1, (float)_projectile.Position.z);
	}

	private void OnDisable()
	{
		_onDisableEffect.gameObject.SetActive(true);
		_onDisableEffect.Play();
	}
}
