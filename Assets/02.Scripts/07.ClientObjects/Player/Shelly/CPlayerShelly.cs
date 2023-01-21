using System.Collections;
using System.Collections.Generic;

using Server.Game;

using UnityEngine;

public class CPlayerShelly : CPlayer
{
	[field: SerializeField] public CShellyBuckShot Buckshot { get; set; }
	[field: SerializeField] public CShellySuperShell SuperShell { get; set; }
	[SerializeField] private AudioClip[] _audio_start;

	public override void Init(NetCharacter character, short teamId)
	{
		base.Init(character, teamId);
		Buckshot.Init(this);
		SuperShell.Init(this);
	}

	public override void OnMatchStart()
	{
		base.OnMatchStart();
		if (User.TeamId == TeamId)
		{
			Audio.PlayOnce(_audio_start[Random.Range(0, _audio_start.Length)]);
		}
	}

	public override void HandleOneFrame()
	{
		if (Active is false)
		{
			return;
		}

		base.HandleOneFrame();
		Animator.SetFloat(AnimatorMeta.Speed_Float, (float)NPlayer.MoveSpeed * (float)NPlayer.TargetMoveDir.magnitude);
		Buckshot.HandleOneFrame();
		SuperShell.HandleOneFrame();
	}
}
