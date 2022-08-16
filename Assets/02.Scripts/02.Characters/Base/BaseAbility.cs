using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAbility : MonoBehaviour
{
	protected BaseCharacter _character;
	protected Coroutine _currentCoroutine;
	protected HitInfo _hitInfo;
	public abstract void Init(BaseCharacter character);
	protected abstract IEnumerator Perform();
}
