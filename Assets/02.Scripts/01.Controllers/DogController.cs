using Server.Game;

public class DogController : CharacterController
{
	protected NetCharacterKnight _dog;

	public override void Init(NetCharacter playableCharacter)
	{
		base.Init(playableCharacter);
		_dog = _currentPlayer as NetCharacterKnight;
	}
}
