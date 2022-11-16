
using Server.Game;

using UnityEngine;

public class CPlayerKnight : CPlayer
{
	[field: SerializeField] public CKnightWhirlwind Whirlwind { get; set; }
	[field: SerializeField] public CKnightBash Bash { get; set; }
	[SerializeField] private Sprite SelectCircleSelf;
	[SerializeField] private Sprite SelectCircleTeamRed;
	[SerializeField] private Sprite SelectCircleTeamBlue;
	[SerializeField] private SpriteRenderer SelectCircle;

	public override void Init(NetCharacter character, short teamId)
	{
		base.Init(character, teamId);
		Whirlwind.Init(this);
		Bash.Init(this);
		if (teamId == User.TeamId)
		{
			SelectCircle.sprite = SelectCircleSelf;
		}
		else if (teamId < 3)
		{
			SelectCircle.sprite = SelectCircleTeamBlue;
		}
		else
		{
			SelectCircle.sprite = SelectCircleTeamRed;
		}
	}

	public override void HandleOneFrame()
	{
		base.HandleOneFrame();
		Animator.SetFloat(AnimatorMeta.Speed_Float, (float)NPlayer.MoveSpeed * (float)NPlayer.TargetMoveDir.magnitude);
		Whirlwind.HandleOneFrame();
		Bash.HandleOneFrame();
	}
}
