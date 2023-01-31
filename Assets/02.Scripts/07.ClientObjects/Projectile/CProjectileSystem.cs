using System;
using System.Collections.Generic;
using System.Linq;

using Server.Game;

using UnityEngine;

using static Enums;

public class CProjectileSystem : CBaseComponentSystem<NetProjectile>
{
	private readonly Dictionary<NetProjectile, CProjectile> _activeDict = new(120);
	private readonly Stack<CProjectile>[] _reservePool = new Stack<CProjectile>[2] { new(90), new(30) };
	private readonly List<CProjectile> _removeArr = new(120);

	[SerializeField]
	private GameObject[] _cProjectilePrefabArr;
	private Transform[] _reservePoolParent;
	private NetProjectileSystem _netSystem;

	public override void Init(NetBaseComponentSystem<NetProjectile> netSystem)
	{
		CProjectile.System = this;
		_netSystem = (NetProjectileSystem)netSystem;
		_reservePoolParent = new Transform[] {
		new GameObject(Enum.GetName(typeof(NetObjectType), NetObjectType.Projectile_Shelly_Buckshot)).transform,
		new GameObject(Enum.GetName(typeof(NetObjectType), NetObjectType.Projectile_Shelly_SuperShell)).transform };
		foreach (var trans in _reservePoolParent)
		{
			trans.parent = transform;
		}

		var query =
			from projectile in netSystem.ComponentDict
			group projectile by projectile.NetObjId.Type into projectileGroup
			select new { Type = projectileGroup.Key, Count = projectileGroup.Count() };
		foreach (var nProjectile in query)
		{
			Reserve(nProjectile.Type, nProjectile.Count);
		}
	}

	public override void OnNetFrameUpdate()
	{
		Loggers.Error.Information("Server : {0}", _netSystem.ActiveSet.Count);
		foreach (var nProjectile in _netSystem.ActiveSet)
		{
			if (_activeDict.ContainsKey(nProjectile) is false)
			{
				var cProjectile = AwakeFromReserve(nProjectile.NetObjId.Type);
				_activeDict.Add(nProjectile, cProjectile);
				cProjectile.Init(nProjectile);
			}
		}
		foreach (var cProjectile in _activeDict.Values)
		{
			cProjectile.OnNetFrameUpdate();
		}

		Interpretate(0);
		Loggers.Error.Information("Client : {0}", _activeDict.Values.Count - _removeArr.Count);

		foreach (var cProjectile in _removeArr)
		{
			Internal_Return(cProjectile);
		}

		_removeArr.Clear();
	}

	public override void Interpretate(float ratio)
	{
		foreach (var cProjectile in _activeDict.Values)
		{
			cProjectile.Interpretate(ratio);
		}
	}

	public override void OnRoundStart()
	{
	}

	public override void OnRoundEnd()
	{
	}

	public override void Clear()
	{
		foreach (var cProjectile in _activeDict.Values)
		{
			cProjectile.Clear();
			_removeArr.Add(cProjectile);
		}

		foreach (var cProjRemove in _removeArr)
		{
			Internal_Return(cProjRemove);
		}

		_removeArr.Clear();
	}

	public override void Reset()
	{
	}

	public void External_Return(CProjectile cProjectile)
	{
		_removeArr.Add(cProjectile);
	}

	private CProjectile AwakeFromReserve(NetObjectType type)
	{
		var index = NetProjectileSystem.GetIndex(type);
		var stack = _reservePool[index];
		return stack.Count != 0 ? _reservePool[index].Pop() :
			Instantiate(_cProjectilePrefabArr[index], _reservePoolParent[index]).GetComponent<CProjectile>();
	}

	private CProjectile Reserve(NetObjectType type)
	{
		var index = NetProjectileSystem.GetIndex(type);
		var cProjectile = Instantiate(_cProjectilePrefabArr[index], _reservePoolParent[index]).GetComponent<CProjectile>();
		_reservePool[index].Push(cProjectile);
		return cProjectile;
	}

	private void Reserve(NetObjectType type, int count)
	{
		var index = NetProjectileSystem.GetIndex(type);
		for (int i = 0; i < count; i++)
		{
			var cProjectile = Instantiate(_cProjectilePrefabArr[index], _reservePoolParent[index]).GetComponent<CProjectile>();
			_reservePool[index].Push(cProjectile);
		}
	}

	private void Internal_Return(CProjectile cProjectile)
	{
		_reservePool[NetProjectileSystem.GetIndex(cProjectile.NProjectile.NetObjId.Type)].Push(cProjectile);
		_activeDict.Remove(cProjectile.NProjectile);
	}
}
