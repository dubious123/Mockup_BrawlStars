using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog_Character : BaseCharacter
{
	#region SerializeFields
	[SerializeField] Dog_Bash _bash;
	#endregion

	public override void Init()
	{
		base.Init();
		_bash.Init(this);
		_currentHp = _maxHp;
	}

	public override void OnHit(HitInfo info)
	{
		base.OnHit(info);
		Debug.Log(_currentHp);
	}
	protected override void OnDead()
	{
		base.OnDead();
		_bash.OnDead();
	}

	public void ChargeBash() => _bash.ChargeBash();
	public void ReleaseBash() => _bash.ReleaseBash();
	public void CancelBash() => _bash.CancelBash();
}
