using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog_Character : BaseCharacter
{
	#region SerializeFields

	#endregion


	protected override void Awake()
	{
		base.Awake();
		_currentHp = _maxHp;
	}

	public override void OnHit(int demage)
	{
		base.OnHit(demage);
		_currentHp = Mathf.Max(0, _currentHp - demage);
		Debug.Log(_currentHp);
		if (_currentHp <= 0) OnDead();
	}
}
