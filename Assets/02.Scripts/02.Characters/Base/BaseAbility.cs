using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public abstract class BaseAbility : MonoBehaviour
{
	protected BaseCharacter _character;
	protected CoroutineHandle _coHandle;
	protected HitInfo _hitInfo;
	public abstract void Init(BaseCharacter character);
	protected abstract IEnumerator<float> Co_Perform();
}
