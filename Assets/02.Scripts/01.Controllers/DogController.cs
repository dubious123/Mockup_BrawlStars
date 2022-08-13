using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DogController : CharacterController
{
	protected Dog_Character _dog;
	protected override void Awake()
	{
		base.Awake();
		_dog = _currentPlayer as Dog_Character;
		_basicAttackAction.started += _ => _dog.ActivateBasicAttack();
		_basicAttackAction.canceled += _ => _dog.DeactivateBasicAttack();
		_abilityQ.started += _ => _dog.ChargeBash();
		_abilityQ.canceled += _ => _dog.ReleaseBash();
		_abilityCancel.performed += _ => _dog.CancelBash();
	}
}
