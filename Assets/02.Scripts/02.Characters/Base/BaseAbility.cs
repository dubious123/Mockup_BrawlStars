using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAbility : MonoBehaviour
{
	protected BaseCharacter _character;
	public abstract void Init(BaseCharacter character);
	protected abstract void OnAbilityStart();
	protected abstract void OnAbilityPerform();
	protected abstract void OnAbilityEnd();
}
