﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Server.Game.Data;
using Server.Game.GameRule;

using Unity.VisualScripting;

using UnityEditor.Rendering.Universal;

using static Enums;

namespace Server.Game
{
	public class NetWorld
	{
		public readonly BaseGameRule GameRule;

		public DetDictionary<NetObjectId, NetObject> NetObjectDict { get; private set; } = new();
		public WorldData Data => _worldData;
		public NTiming NetTiming = new();
		public NetObjectBuilder ObjectBuilder { get; }
		public NetCollider2DSystem ColliderSystem { get; }
		public NetCharacterSystem CharacterSystem { get; }
		public NetEnvSystem EnvSystem { get; }
		public NetProjectileSystem ProjectileSystem { get; }
		public NetCharacter[] NetCharacters = new NetCharacter[6];
		public bool Active { get; set; } = true;
		public bool AllowInput { get; set; } = true;

		private readonly WorldData _worldData;

		public NetWorld(WorldData data, BaseGameRule gameRule)
		{
			GameRule = gameRule;
			GameRule.World = this;
			ObjectBuilder = new() { World = this };
			ColliderSystem = new() { World = this };
			CharacterSystem = new() { World = this };
			EnvSystem = new() { World = this };
			ProjectileSystem = new() { World = this };
			_worldData = data;
		}

		public void SetNetObjectActive(NetObject netObj, bool active)
		{
			ColliderSystem.SetActive(netObj, active);
			CharacterSystem.SetActive(netObj, active);
			EnvSystem.SetActive(netObj, active);
			ProjectileSystem.SetActive(netObj, active);
		}

		public void Reset()
		{
			Active = true;
			ColliderSystem.Reset();
			CharacterSystem.Reset();
			EnvSystem.Reset();
			ProjectileSystem.Reset();
		}

		public void UpdateInputs(GameFrameInfo inputInfo)
		{
			if (AllowInput is false)
			{
				inputInfo.Reset();
			}

			foreach (var player in CharacterSystem.ComponentDict)
			{
				player.UpdateInput(inputInfo.Inputs[player.NetObj.ObjectId.InstanceId]);
			}
		}

		public void Update()
		{
			if (Active is false)
			{
				return;
			}

			NetTiming.Update();
			ColliderSystem.Update();
			CharacterSystem.Update();
			EnvSystem.Update();
			ProjectileSystem.Update();
			GameRule.Update();
		}

		public void AddNewNetObject(NetObject obj)
		{
			NetObjectDict.Add(obj.ObjectId, obj);
		}

		public void RemoveNetObject(NetObject obj)
		{
			NetObjectDict.Remove(obj.ObjectId);
			ColliderSystem.RemoveComponent(obj);
			CharacterSystem.RemoveComponent(obj);
			EnvSystem.RemoveComponent(obj);
			ProjectileSystem.RemoveComponent(obj);
		}

		public NetObject FindNetObject(NetObjectId inGameId) => NetObjectDict[inGameId];

		public void FindNetObjects(Func<NetObject, bool> condition, IList<NetObject> result)
		{
			foreach (var obj in NetObjectDict)
			{
				if (condition(obj))
				{
					result.Add(obj);
				}
			}
		}

		public bool FindAllAndBroadcast(Func<NetObject, bool> condition, Action<NetObject> action)
		{
			var count = 0;
			foreach (var obj in NetObjectDict)
			{
				if (condition(obj))
				{
					count++;
					action(obj);
				}
			}

			return count > 0;
		}
	}
}

