using UnityEngine;

public class Dog_Character : BaseCharacter
{
	#region SerializeFields
	[SerializeField] private Dog_Bash _bash;
	#endregion

	public override void Init()
	{
		base.Init();
		_bash.Init(this);
		_currentHp = _maxHp;
	}

	public override void HandleInput(ref Vector2 moveDir, ref Vector2 lookDir, ushort buttonPressed)
	{
		base.HandleInput(ref moveDir, ref lookDir, buttonPressed);

		_bash.HandleInput(((buttonPressed >> 1) & 1) == 1);
		if (((buttonPressed >> 2) & 1) == 1)
		{

		}
	}

	public override void SetOtherSkillsActive(uint skillId, bool active)
	{
		base.SetOtherSkillsActive(skillId, active);
		if (_bash.Id == skillId == false) _bash.SetActive(active);
	}

	public override void OnGetHit(in HitInfo info)
	{
		base.OnGetHit(info);
		Debug.Log(_currentHp);
	}

	protected override void HandleSkills()
	{
		base.HandleSkills();
		_bash.HandleOneFrame();
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
