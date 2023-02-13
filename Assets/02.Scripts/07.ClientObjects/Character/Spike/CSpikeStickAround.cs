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

	private NSpikeStickAround _netStickAround;

	public void Init(ClientCharacterSpike spike)
	{
		Character = spike;
		_netStickAround = (spike.NPlayer as NCharacterSpike).SpecialAttack as NSpikeStickAround;
		_hudPowerCircle.Init(_netStickAround);
		if (spike.TeamId == User.TeamId)
		{
			GameInput.PowerInputAction.started += OnPressed;
			GameInput.PowerInputAction.canceled += OnReleased;
		}
	}

	public void HandleAttack()
	{
		Audio.PlayOnce(_audio, 0.5f);
		Audio.PlayerPowerPerformed();
		Character.Animator.SetBool(AnimatorMeta.IsAttack, true);
		//Character.PlayerEffect.PlaySpecialAttackEffect();
	}

	public void Reset()
	{
		_hudPowerCircle.Reset();
	}

	private void OnPressed(InputAction.CallbackContext _)
	{
		if (_netStickAround.CanAttack())
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
		GameInput.PowerInputAction.started -= OnPressed;
		GameInput.PowerInputAction.canceled -= OnReleased;
	}
}
