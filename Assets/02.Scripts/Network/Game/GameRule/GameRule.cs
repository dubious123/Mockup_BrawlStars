using System;

using static Enums;

namespace Server.Game.GameRule
{
	public abstract class BaseGameRule
	{
		public NetWorld World { get; set; }
		public abstract TeamType GetTeamType(uint objectId);
		public abstract bool CanSendHit(INetObject from, INetObject to);
		public abstract void UpdateGameLogic();
	}
}
