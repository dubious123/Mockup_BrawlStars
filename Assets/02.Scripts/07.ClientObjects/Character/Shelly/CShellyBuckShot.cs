using MEC;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CShellyBuckShot : MonoBehaviour
{
	[SerializeField] private Graphic _indicator;
	[SerializeField] private GameObject _bulletPrefabBlue;
	[SerializeField] private GameObject _bulletPrefabRed;
	[SerializeField] private AudioClip _audio;
	[SerializeField] private Color _red;
	[SerializeField] private Color _white;

	public bool Performing { get; set; }
	public bool Active { get; set; }
	public CPlayerShelly Player { get; set; }

	private NShellyBuckShot _netBuckShot;
	private CoroutineHandle _coHandle;

	public void Init(CPlayerShelly shelly)
	{
		Player = shelly;
		_netBuckShot = (shelly.NPlayer as NCharacterShelly).BasicAttack as NShellyBuckShot;
		if (shelly.TeamId == User.TeamId)
		{
			GameInput.BasicAttackInputAction.started += OnPressed;
			GameInput.BasicAttackInputAction.canceled += OnReleased;
			_coHandle = Timing.CallContinuously(float.MaxValue, () => _indicator.color = _netBuckShot.CurrentShellCount > 0 ? _white : _red);
		}

		var bulletPrefab = shelly.NPlayer.Team == User.Team ? _bulletPrefabBlue : _bulletPrefabRed;
		foreach (var pallet in _netBuckShot.Shots)
		{
			var cBullet = Instantiate(bulletPrefab, transform).GetComponent<CProjectile>();
			(Scene.CurrentScene as Scene_Map1).ClientGameLoop.ProjectileSystem.AddAndInitProjectile(cBullet, pallet);
		}
	}

	public void HandleAttack()
	{
		Audio.PlayOnce(_audio);
	}

	private void OnPressed(InputAction.CallbackContext _)
	{
		_indicator.gameObject.SetActive(true);
	}

	private void OnReleased(InputAction.CallbackContext _)
	{
		_indicator.gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		GameInput.BasicAttackInputAction.started -= OnPressed;
		GameInput.BasicAttackInputAction.canceled -= OnReleased;
		Timing.KillCoroutines(_coHandle);
	}
}