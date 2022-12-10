using System.Collections.Generic;

using Server.Game;

using UnityEngine;

using static Enums;

public class NetProjectile : INetUpdatable, INetObject
{
	public NetCharacter Owner { get; set; }
	public bool IsAlive { get; private set; }
	public sVector3 Position { get; set; }
	public sQuaternion Rotation { get; set; }
	public sfloat Speed { get; set; }
	public INetCollider2D Collider { get; init; }
	public sVector3 Velocity { get; private set; }

	public uint ObjectId { get; init; }
	public NetObjectTag Tag { get; set; }
	public NetWorld World { get; init; }

	private sVector3 _moveDir;
	private readonly HitInfo _hitInfo;
	private readonly int _maxTravelTime;
	private int _currentTravelTime;

	public NetProjectile(NetCharacter character, sfloat range, sfloat speed, HitInfo hitInfo, int maxTravelTime, sVector3 moveDir)
	{
		Owner = character;
		Collider = new NetCircleCollider2D(this, sVector2.zero, range);
		_hitInfo = hitInfo;
		Speed = speed;
		_moveDir = moveDir;
		_maxTravelTime = maxTravelTime;
		_currentTravelTime = 0;
		Disable();
	}

	public void Disable()
	{
		IsAlive = false;
	}

	public void Reset(sVector3 pos, sQuaternion rot)
	{
		IsAlive = true;
		Position = pos;
		Rotation = rot;
		Velocity = rot * _moveDir * Speed;
		_currentTravelTime = 0;
	}

	public void Update()
	{
		if (IsAlive == false)
		{
			return;
		}

		if (_currentTravelTime >= _maxTravelTime)
		{
			Disable();
			return;
		}

		Position += Velocity;
		Owner.World.FindAllAndBroadcast(target =>
		{
			if (target == Owner || target is not INetCollidable2D)
			{
				return false;
			}

			var co = target as INetCollidable2D;
			return co.Collider.CheckCollision(Collider);
		}, target =>
		{
			if (target is ITakeHit)
			{
				Owner.SendHit(target as ITakeHit, _hitInfo);
			}

			Disable();
		});

		++_currentTravelTime;
	}
}
