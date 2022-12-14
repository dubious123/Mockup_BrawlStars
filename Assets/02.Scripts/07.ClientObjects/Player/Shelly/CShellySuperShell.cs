using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CShellySuperShell : MonoBehaviour, ICBaseSkill
{
	[SerializeField] private GameObject _indicator;
	[SerializeField] private GameObject _bulletPrefabBlue;
	[SerializeField] private GameObject _bulletPrefabRed;
	[SerializeField] private ParticleSystem _effect;
	[SerializeField] private AudioClip _audio;

	public bool Performing { get; set; }
	public bool Active { get; set; }
	public CPlayerShelly Player { get; set; }

	private List<CProjectile> _cBullets;
	private NShellySuperShell _netSuperShell;
	private GameObject _bulletPrefab;

	public void Init(CPlayerShelly shelly)
	{
		Player = shelly;
		_netSuperShell = (shelly.NPlayer as NCharacterShelly).SuperShell as NShellySuperShell;
		_cBullets = new List<CProjectile>(_netSuperShell.PalletsCountPerShot);
		if (shelly.NPlayer.Team == User.Team)
		{
			_bulletPrefab = _bulletPrefabBlue;
		}
		else
		{
			_bulletPrefab = _bulletPrefabRed;
		}

		foreach (var pallet in _netSuperShell.Pallets)
		{
			var cBullet = Instantiate(_bulletPrefab, transform).GetComponent<CProjectile>();
			cBullet.Init(pallet);
			_cBullets.Add(cBullet);
		}
	}

	public void HandleOneFrame()
	{
		_indicator.SetActive(_netSuperShell.Holding);
		if (_netSuperShell.IsAttack is true)
		{
			Player.Animator.SetBool(AnimatorMeta.IsAttack, true);
			_cBullets.ForEach(bullet => bullet.enabled = bullet.IsAlive);
			_effect.gameObject.SetActive(true);
			_effect.Play();
			Audio.PlayOnce(_audio);
			return;
		}

		Player.Animator.SetBool(AnimatorMeta.IsAttack, false);
	}
}
