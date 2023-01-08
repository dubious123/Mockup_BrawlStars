using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Unity.VisualScripting;

using UnityEditor;

using UnityEngine;
using UnityEngine.UI;

public class CShellyBuckShot : MonoBehaviour, ICBaseSkill
{
	[SerializeField] private GameObject _indicator;
	[SerializeField] private GameObject _bulletPrefabBlue;
	[SerializeField] private GameObject _bulletPrefabRed;
	[SerializeField] private AudioClip _audio;

	public bool Performing { get; set; }
	public bool Active { get; set; }
	public CPlayerShelly Player { get; set; }

	private List<CProjectile> _cBullets;
	private NShellyBuckShot _netBuckShot;
	private GameObject _bulletPrefab;

	public void Init(CPlayerShelly shelly)
	{
		Player = shelly;
		_netBuckShot = (shelly.NPlayer as NCharacterShelly).BuckShot as NShellyBuckShot;
		_cBullets = new List<CProjectile>(_netBuckShot.AmmoCount * _netBuckShot.BulletAmountPerAttack);
		if (shelly.NPlayer.Team == User.Team)
		{
			_bulletPrefab = _bulletPrefabBlue;
		}
		else
		{
			_bulletPrefab = _bulletPrefabRed;
		}

		foreach (var pallet in _netBuckShot.Shots)
		{
			var cBullet = Instantiate(_bulletPrefab, transform).GetComponent<CProjectile>();
			cBullet.Init(pallet);
			_cBullets.Add(cBullet);
		}
	}

	public void HandleOneFrame()
	{
		_indicator.SetActive(_netBuckShot.Holding);

		if (_netBuckShot.IsAttack is true)
		{
			Player.Animator.SetBool(AnimatorMeta.IsAttack, true);
			_cBullets.ForEach(bullet => bullet.enabled = bullet.IsAlive);
			Audio.PlayOnce(_audio);
			return;
		}

		Player.Animator.SetBool(AnimatorMeta.IsAttack, false);
	}
}