using System;

using Server.Game;

using UnityEngine.ResourceManagement.Util;

public class GameFrameInfo
{
	private GameFrameInfo() { }

	public GameFrameInfo(S_GameFrameInfo req)
	{
		FrameNum = req.FrameNum;
		Inputs = new InputData[req.PlayerMoveDirXArr.Length];

		var length = req.PlayerMoveDirXArr.Length;
		for (int i = 0; i < length; i++)
		{
			Inputs[i] = new InputData()
			{
				MoveInput = new sVector3(sfloat.FromRaw(req.PlayerMoveDirXArr[i]), sfloat.Zero, sfloat.FromRaw(req.PlayerMoveDirYArr[i])),
				LookInput = new sVector3(sfloat.FromRaw(req.PlayerLookDirXArr[i]), sfloat.Zero, sfloat.FromRaw(req.PlayerLookDirYArr[i])),
				ButtonInput = req.ButtonPressedArr[i]
			};
		}
	}

	public static GameFrameInfo GetDefault(int playerNum)
	{
		return new GameFrameInfo()
		{
			Inputs = new InputData[playerNum],
		};
	}

	public InputData[] Inputs { get; private set; }
	public int FrameNum { get; private set; }

	public void Reset()
	{
		for (int i = 0; i < Inputs.Length; i++)
		{
			Inputs[i] = default;
		}
	}
}
