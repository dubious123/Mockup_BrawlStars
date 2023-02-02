using UnityEngine;
using UnityEngine.InputSystem;

public class CShellySuperShell : MonoBehaviour
{
	[SerializeField] private GameObject _indicator;
	[SerializeField] private AudioClip _audio;
	[SerializeField] private HudPowerCircle _hudPowerCircle;

	public bool Active { get; set; }
	public CPlayerShelly Player { get; set; }

	private NShellySuperShell _netSuperShell;

	public void Init(CPlayerShelly shelly)
	{
		Player = shelly;
		_netSuperShell = (shelly.NPlayer as NCharacterShelly).SpecialAttack as NShellySuperShell;
		_hudPowerCircle.Init(_netSuperShell);
		if (shelly.TeamId == User.TeamId)
		{
			GameInput.PowerInputAction.started += OnPressed;
			GameInput.PowerInputAction.canceled += OnReleased;
		}
	}

	public void HandleAttack()
	{
		Player.Animator.SetBool(AnimatorMeta.IsAttack, true);
		Player.PlayerEffect.PlaySpecialAttackEffect();
	}

	public void Reset()
	{
		_hudPowerCircle.Reset();
	}

	private void OnPressed(InputAction.CallbackContext _)
	{
		if (_netSuperShell.CanAttack())
		{
			_indicator.SetActive(true);
		}
	}

	private void OnReleased(InputAction.CallbackContext _)
	{
		_indicator.SetActive(false);
	}

	private void OnDestroy()
	{
		GameInput.BasicAttackInputAction.started -= OnPressed;
		GameInput.BasicAttackInputAction.canceled -= OnReleased;
	}
}
