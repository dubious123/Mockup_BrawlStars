using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public abstract class BaseAbility : MonoBehaviour
{
	protected BaseCharacter _character;
	protected HitInfo _hitInfo;
	protected CoroutineHandle _coHandle;
	public abstract void Init(BaseCharacter character);
	public abstract void OnDead();
	protected abstract IEnumerator<float> Co_Perform();
}
