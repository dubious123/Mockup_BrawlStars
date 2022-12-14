using System.Collections.Generic;
using System.Linq;

using Server.Game;

using UnityEngine;

public class NShellyBuckShot : NetBaseSkill
{
	public bool Holding { get; private set; }
	public bool IsAttack { get; private set; }
	public int AmmoCount => _ammoCount;
	public int BulletAmountPerAttack => _bulletAmountPerAttack;
	public sfloat BulletAngle => _bulletAngle;
	public LinkedList<NetProjectile[]> AliveBulletArrArr { get; private set; }
	public Queue<NetProjectile[]> BulletArrQueue { get; private set; }

	private IEnumerator<int> _coHandler;
	private readonly NCharacterShelly _shelly;
	private readonly HitInfo _hitInfo;
	private int _ammoCount, _maxAmmoCount, _bulletAmountPerAttack, _reloadFrame, _currentReloadDeltaFrame,
		_waitFrameBeforePerform, _waitFrameAfterPerform, _bulletMaxTravelTime;
	private sfloat _bulletCollisionRange, _bulletSpeed, _bulletAngle;

	private bool _nowPressed, _beforePressed;

	public NShellyBuckShot(NCharacterShelly character)
	{
		_shelly = character;
		Id = 0x11;
		_maxAmmoCount = 3;
		_ammoCount = _maxAmmoCount;
		_bulletAmountPerAttack = 5;
		_reloadFrame = 60;
		_waitFrameBeforePerform = 10;
		_waitFrameAfterPerform = 10;
		_bulletMaxTravelTime = 48;
		_bulletCollisionRange = (sfloat)0.2f;
		_bulletSpeed = (sfloat)10 / (sfloat)_bulletMaxTravelTime;
		_bulletAngle = (sfloat)30f;
		_hitInfo = new HitInfo()
		{
			Damage = 20,
		};

		AliveBulletArrArr = new();
		BulletArrQueue = new Queue<NetProjectile[]>(_maxAmmoCount);
		var degreeOffset = 90 - _bulletAngle * 0.5f;
		var degreeDelta = _bulletAngle / _bulletAmountPerAttack;
		for (int i = 0; i < _maxAmmoCount; i++)
		{
			var arr = new NetProjectile[_bulletAmountPerAttack];
			for (int j = 0; j < _bulletAmountPerAttack; ++j)
			{
				var angle = (degreeOffset + degreeDelta * j) * sMathf.Deg2Rad;
				arr[j] = new NetProjectile(_shelly, _bulletCollisionRange, _bulletSpeed, _hitInfo, _bulletMaxTravelTime, new sVector3(sMathf.Cos(angle), sfloat.Zero, sMathf.Sin(angle)));
			}

			BulletArrQueue.Enqueue(arr);
		}
	}

	public override void Update()
	{
		if (Active is false)
		{
			return;
		}

		HandleInputInternal();

		UpdateBullets();

		if (_currentReloadDeltaFrame < _reloadFrame)
		{
			++_currentReloadDeltaFrame;
		}
		else if (_ammoCount < _maxAmmoCount)
		{
			_currentReloadDeltaFrame = 0;
			++_ammoCount;
		}
	}

	public override void HandleInput(in InputData input)
	{
		_beforePressed = _nowPressed;
		_nowPressed = (input.ButtonInput & 1) == 1;
	}

	protected override IEnumerator<int> Co_Perform()
	{
		_shelly.CanControlMove = false;
		_shelly.CanControlLook = false;
		for (int i = 0; i < _waitFrameBeforePerform; i++)
		{
			yield return 0;
		}

		IsAttack = true;
		var bullets = BulletArrQueue.Dequeue();
		AliveBulletArrArr.AddLast(bullets);

		for (int i = 0; i < _bulletAmountPerAttack; i++)
		{
			var bullet = bullets[i];

			bullet.Reset(_shelly.Position, _shelly.Rotation);
		}

		yield return 0;
		IsAttack = false;
		for (int i = 0; i < _waitFrameAfterPerform; i++)
		{
			yield return 0;
		}

		_shelly.CanControlMove = true;
		_shelly.CanControlLook = true;
		_shelly.SetActiveOtherSkills(this, true);
		Performing = false;
		yield break;
	}

	public void UpdateBullets()
	{
		for (var arr = AliveBulletArrArr.First; arr is not null;)
		{
			var isAlive = false;
			foreach (var bullet in arr.Value)
			{
				bullet.Update();
				isAlive |= bullet.IsAlive;
			}

			if (isAlive)
			{
				arr = arr.Next;
				continue;
			}

			var remove = arr;
			arr = arr.Next;
			AliveBulletArrArr.Remove(remove);
			BulletArrQueue.Enqueue(remove.Value);
		}
	}

	public override void Cancel()
	{
		throw new System.NotImplementedException();
	}

	private void HandleInputInternal()
	{
		if (Performing is true)
		{
			_coHandler.MoveNext();
			return;
		}

		Holding = _beforePressed is true && _nowPressed is true && _ammoCount > 0;
		if (_beforePressed is true && _nowPressed is false && _ammoCount > 0)
		{
			_shelly.SetActiveOtherSkills(this, false);
			Performing = true;
			_coHandler = Co_Perform();
			--_ammoCount;
		}
	}
}
