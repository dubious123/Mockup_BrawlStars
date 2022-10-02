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
	public override void HandleOneFrame()
	{
		base.HandleOneFrame();
		_bash.HandleOneFrame();
	}
	public override void HandleInput(ref Vector2 moveDir, ref Vector2 lookDir, ushort mousePressed)
	{
		base.HandleInput(ref moveDir, ref lookDir, mousePressed);

		_bash.HandleInput(((mousePressed >> 1) & 1) == 1);
		if (((mousePressed >> 2) & 1) == 1)
		{

		}
	}
	public override void SetOtherSkillsActive(uint skillId, bool active)
	{
		base.SetOtherSkillsActive(skillId, active);
		if (_bash.Id == skillId == false) _bash.SetActive(active);
	}
	public override void OnGetHit(HitInfo info)
	{
		base.OnGetHit(info);
		Debug.Log(_currentHp);
	}
	protected override void OnDead()
	{
		base.OnDead();
		//_bash.OnDead();
	}

	//public void ChargeBash() => _bash.ChargeBash();
	//public void ReleaseBash() => _bash.ReleaseBash();
	//public void CancelBash() => _bash.CancelBash();
}
