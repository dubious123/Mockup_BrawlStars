using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using System;

public abstract class BaseSkill : MonoBehaviour
{
	public uint Id;
	protected BaseCharacter _character;
	protected AnimationClip _clip;
	protected HitInfo _hitInfo;
	protected bool _enabled = true;
	public virtual void Init(BaseCharacter character)
	{
		_character = character;
		Id = 1;
	}
	public abstract void HandleInput(bool buttonPressed);
	public abstract void HandleOneFrame();
	public abstract void Cancel();
	public virtual void SetActive(bool set)
	{
		_enabled = set;
	}
}
