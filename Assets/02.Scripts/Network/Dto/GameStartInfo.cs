using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Enums;

public class GameStartInfo
{
	public GameStartInfo(S_BroadcastStartGame req)
	{
		CharacterType = new NetObjectType[req.CharacterTypeArr.Length];
		for (int i = 0; i < req.CharacterTypeArr.Length; i++)
		{
			CharacterType[i] = (NetObjectType)req.CharacterTypeArr[i];
		}
	}

	public NetObjectType[] CharacterType { get; init; }
}
