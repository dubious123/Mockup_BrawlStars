public class DogController : CharacterController
{
	protected Dog_Character _dog;

	public override void Init(BaseCharacter playableCharacter)
	{
		base.Init(playableCharacter);
		_dog = _currentPlayer as Dog_Character;
	}
}
