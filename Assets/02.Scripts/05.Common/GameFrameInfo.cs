using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using static S_BroadcastGameState;

public class GameFrameInfo
{
	public GameFrameInfo(S_BroadcastGameState req, Func<IEnumerable<GameActionResult>, IEnumerable<GameActionContext>> contextFactory)
	{
		_req = req;
		ActionContexts = contextFactory.Invoke(_req.Actions).ToList();
	}
	[SerializeField] private S_BroadcastGameState _req;
	public long StartTick => _req.StartTick;
	public long TargetTick => _req.TargetTick;
	public Vector2[] MoveInput => _req.PlayerMoveDirArr;
	public Vector2[] LookInput => _req.PlayerLookDirArr;
	public ushort[] ButtonPressed => _req.ButtonPressedArr;
	public List<GameActionContext> ActionContexts { get; init; }

	public class GameActionContext
	{
		public uint ActionCode;
		public BaseCharacter Subject;
		public BaseCharacter[] Objects;
	}

	public void ToString(StringBuilder sb)
	{
		if (sb is null) return;
		sb.AppendLine("{");
		sb.AppendLine($"StartTick : {StartTick}");
		sb.AppendLine($"TargetTick : {TargetTick}");
		sb.Append("MoveInput : [");
		foreach (var vector in MoveInput)
		{
			sb.Append($"({vector.x},{vector.y})");
		}
		sb.AppendLine("]");
		sb.Append("LookInput : [");
		foreach (var vector in LookInput)
		{
			sb.Append($"({vector.x},{vector.y})");
		}
		sb.AppendLine("]");
		sb.Append("ButtonPressed : [");
		foreach (var item in ButtonPressed)
		{
			sb.Append(item.ToString());
		}
		sb.AppendLine("]");
		sb.Append("GameActionContext : [");
		foreach (var ctx in ActionContexts)
		{
			sb.Append($"Code : {ctx.ActionCode}, Subject : {ctx.Subject.TeamId}, Objects : [");
			foreach (var obj in ctx.Objects)
			{
				sb.Append($"{obj.TeamId},");
			}
			sb.Append("]");
		}
		sb.AppendLine("]");
		sb.AppendLine("}");
	}
}
