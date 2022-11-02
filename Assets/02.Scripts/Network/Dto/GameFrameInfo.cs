using Server.Game;

public class GameFrameInfo
{
	public GameFrameInfo(S_GameFrameInfo req)
	{
		_req = req;
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

	private S_GameFrameInfo _req;
	public long StartTick => _req.StartTick;
	public long TargetTick => _req.TargetTick;
	public InputData[] Inputs { get; private set; }
}
