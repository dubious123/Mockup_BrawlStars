using static Enums;

namespace Server.Game.GameRule
{
	public class GameRule00 : IGameRule
	{
		public TeamType GetTeamType(uint objectId)
		{
			if (objectId < 6)
			{
				return objectId % 2 == 0 ? TeamType.Blue : TeamType.Red;
			}

			return TeamType.None;
		}

		public bool CanSendHit(INetObject from, INetObject to)
		{
			if (from is NetCharacter && to is NetCharacter)
			{
				return (from as NetCharacter).Team != (to as NetCharacter).Team;
			}

			return to is INetCollidable2D and ITakeHit;
		}
	}
}
