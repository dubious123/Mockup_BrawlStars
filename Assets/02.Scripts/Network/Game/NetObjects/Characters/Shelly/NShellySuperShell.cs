using System.Collections.Generic;

using Server.Game;

public class NShellySuperShell : NetBaseSkill
{
	public bool BtnPressed { get; private set; }
	public int BulletAmountPerAttack => _bulletAmountPerAttack;
	public sfloat BulletAngle => _bulletAngle;
	public NetProjectile[] Bullets;

	private IEnumerator<int> _coHandler;
	private readonly NCharacterShelly _shelly;
	private readonly HitInfo _hitInfo;
	private int _bulletAmountPerAttack, _reloadFrame, _currentReloadDeltaFrame,
		_waitFrameBeforePerform, _waitFrameAfterPerform, _bulletMaxTravelTime;
	private sfloat _bulletCollisionRange, _bulletSpeed, _bulletAngle;

	public NShellySuperShell(NCharacterShelly character)
	{
		_shelly = character;
		Character = character;
		Id = 0x11;
		_bulletAmountPerAttack = 10;
		_reloadFrame = 600;
		_currentReloadDeltaFrame = 0;
		_waitFrameBeforePerform = 20;
		_waitFrameAfterPerform = 20;
		_bulletMaxTravelTime = 60;
		_bulletCollisionRange = (sfloat)0.2f;
		_bulletSpeed = (sfloat)10 / (sfloat)_bulletMaxTravelTime;
		_hitInfo = new HitInfo()
		{
			Damage = 25,
			KnockbackDistance = (sfloat)1f,
			KnockbackDuration = 15
		};

		Bullets = new NetProjectile[_bulletAmountPerAttack];
		for (int j = 0; j < _bulletAmountPerAttack; ++j)
		{
			//Bullets[j] = new NetProjectile(_shelly, _bulletCollisionRange, _bulletSpeed, _hitInfo, _bulletMaxTravelTime);
		}
	}

	public override void Update()
	{
		//if (Active is false)
		//{
		//	return;
		//}

		//if (Performing is true)
		//{
		//	_coHandler.MoveNext();
		//}

		//foreach (var bullet in Bullets)
		//{
		//	if (bullet.IsAlive)
		//	{
		//		bullet.Update();
		//	}
		//}

		//if (_currentReloadDeltaFrame < _reloadFrame && (Performing is false))
		//{
		//	++_currentReloadDeltaFrame;
		//}
	}

	public override void HandleInput(in InputData input)
	{
		//var nowPressed = (input.ButtonInput & 0) == 1;
		//if (BtnPressed is true && nowPressed is false && _currentReloadDeltaFrame >= _reloadFrame)
		//{
		//	_shelly.SetActiveOtherSkills(this, false);
		//	Performing = true;
		//	_coHandler = Co_Perform();
		//	_currentReloadDeltaFrame = 0;
		//	return;
		//}

		//BtnPressed = nowPressed;
	}

	protected override IEnumerator<int> Co_Perform()
	{
		for (int i = 0; i < _waitFrameBeforePerform; i++)
		{
			yield return 0;
		}

		foreach (var bullet in Bullets)
		{
			bullet.Reset(_shelly.Position, _shelly.Rotation);
		}

		for (int i = 0; i < _waitFrameAfterPerform; i++)
		{
			yield return 0;
		}

		_shelly.SetActiveOtherSkills(this, true);
		Performing = false;
		_currentReloadDeltaFrame = 0;
		yield break;
	}

	public override void Cancel()
	{
		throw new System.NotImplementedException();
	}
}
