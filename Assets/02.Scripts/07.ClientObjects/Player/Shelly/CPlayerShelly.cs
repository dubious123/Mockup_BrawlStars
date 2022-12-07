using System.Collections;
using System.Collections.Generic;

using Server.Game;

using UnityEngine;

public class CPlayerShelly : CPlayer
{
	[field: SerializeField] public CShellyBuckShot Buckshot { get; set; }
	[field: SerializeField] public CShellySuperShell SuperShell { get; set; }

	public override void Init(NetCharacter character, short teamId)
	{
		base.Init(character, teamId);
		Buckshot.Init(this);
		SuperShell.Init(this);
	}

	public override void HandleOneFrame()
	{
		base.HandleOneFrame();
		Animator.SetFloat(AnimatorMeta.Speed_Float, (float)NPlayer.MoveSpeed * (float)NPlayer.TargetMoveDir.magnitude);
		Buckshot.HandleOneFrame();
		SuperShell.HandleOneFrame();
	}
}
