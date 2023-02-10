using Server.Game;

using UnityEngine;
using UnityEngine.InputSystem;

public class CSpikeStickAround : MonoBehaviour
{
	[SerializeField] private GameObject _indicator;
	[SerializeField] private AudioClip _audio;
	[SerializeField] private HudPowerCircle _hudPowerCircle;

	public bool Active { get; set; }
	public ClientCharacterSpike Character { get; set; }

	private NSpikeStickAround _netSuperShell;

	public void Init(ClientCharacterSpike spike)
	{
		Character = spike;
		_netSuperShell = (spike.NPlayer as NCharacterShelly).SpecialAttack as NSpikeStickAround;
		_hudPowerCircle.Init(_netSuperShell);
		if (spike.TeamId == User.TeamId)
		{
			GameInput.PowerInputAction.started += OnPressed;
			GameInput.PowerInputAction.canceled += OnReleased;
		}
	}

	public void HandleAttack()
	{
		Audio.PlayOnce(_audio);
		Character.Animator.SetBool(AnimatorMeta.IsAttack, true);
		Character.PlayerEffect.PlaySpecialAttackEffect();
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
