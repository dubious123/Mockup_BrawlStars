using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog_Character : BaseCharacter
{
	#region SerializeFields
	[SerializeField] Dog_Bash _bash;
	#endregion


	protected override void Awake()
	{
		base.Awake();
		_bash.Init(this);
		_currentHp = _maxHp;
	}

	public override void OnHit(HitInfo info)
	{
		base.OnHit(info);
		Debug.Log(_currentHp);
	}

	public void ChargeBash() => _bash.ChargeBash();
	public void ReleaseBash() => _bash.ReleaseBash();
	public void CancelBash() => _bash.CancelBash();
}
