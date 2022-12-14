using System;
using System.Collections.Generic;
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

		public Dictionary<NetObjectId, NetObject> NetObjectDict { get; private set; } = new();
		public NetObjectBuilder ObjectBuilder { get; }
		public NetCollider2DSystem ColliderSystem { get; }
		public NetCharacterSystem CharacterSystem { get; }
		public NetEnvSystem EnvSystem { get; }
		public NetProjectileSystem ProjectileSystem { get; }
		public GameFrameInfo InputInfo { get; set; }
		public NetCharacter[] NetCharacters = new NetCharacter[6];

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
			foreach (var netObjData in data.NetObjectDatas)
			{
				var obj = ObjectBuilder.GetNewObject(NetObjectType.Env_Wall)
					.SetPositionAndRotation(netObjData.Position, netObjData.Rotation);
				var collider = obj.GetComponent<NetBoxCollider2D>();
				collider.SetOffsetAndSize(netObjData.BoxCollider.Offset, netObjData.BoxCollider.Size);
			}
		}

		public void OnWorldStart()
		{
			foreach (var character in CharacterSystem.ComponentDict.Values)
			{
				character.Position = _worldData.SpawnPoints[character.NetObjId.InstanceId];
			}
		}

		public void Update()
		{
			UpdateGameLogic();
			UpdateInputs();
			ColliderSystem.Update();
			CharacterSystem.Update();
			EnvSystem.Update();
			ProjectileSystem.Update();
		}

		public void UpdateGameLogic()
		{
			GameRule.UpdateGameLogic();
		}

		public void UpdateInputs()
		{
			foreach (var player in CharacterSystem.ComponentDict.Values)
			{
				if (player is null || player.Active is false)
				{
					continue;
				}

				Loggers.Game.Information("Player [{0}]", player.NetObj.ObjectId.InstanceId);
				player.UpdateInput(InputInfo.Inputs[player.NetObj.ObjectId.InstanceId]);
				Loggers.Game.Information("Position [{0:x},{1:x},{2:x}]] : ", player.Position.x.RawValue, player.Position.y.RawValue, player.Position.z.RawValue);
			}
		}

		public void AddNewNetObject(NetObject obj)
		{
			NetObjectDict.Add(obj.ObjectId, obj);
		}

		public NetObject FindNetObject(NetObjectId inGameId) => NetObjectDict[inGameId];

		public void FindNetObjects(Func<NetObject, bool> condition, IList<NetObject> result)
		{
			foreach (var obj in NetObjectDict.Values)
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
			foreach (var obj in NetObjectDict.Values)
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

