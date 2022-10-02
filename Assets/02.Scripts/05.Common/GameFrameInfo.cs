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
	public GameFrameInfo(S_BroadcastGameState req, Func<IEnumerable<GameActionInfo>, IEnumerable<GameActionContext>> contextFactory)
	{
		_req = req;
		ActionContexts = contextFactory.Invoke(_req.Actions).ToList();
	}
	[SerializeField] S_BroadcastGameState _req;
	public long StartTick => _req.StartTick;
	public long TargetTick => _req.TargetTick;
	public Vector2[] MoveInput => _req.PlayerMoveDirArr;
	public Vector2[] LookInput => _req.PlayerLookDirArr;
	public ushort[] ButtonPressed => _req.ButtonPressedArr;
	public List<GameActionContext> ActionContexts;
	public class GameActionContext
	{
		public uint ActionCode;
		public BaseCharacter Subject;
		public BaseCharacter[] Objects;
	}
}
