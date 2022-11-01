using Server.Game;

public class DogController : CharacterController
{
	protected NetCharacterDog _dog;

	public override void Init(NetCharacter playableCharacter)
	{
		base.Init(playableCharacter);
		_dog = _currentPlayer as NetCharacterDog;
	}
}
