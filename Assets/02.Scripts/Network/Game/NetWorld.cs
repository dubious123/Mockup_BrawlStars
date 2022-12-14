using System;
using System.Collections.Generic;
using System.Linq;

using Server.Game.Data;
using Server.Game.GameRule;

using UnityEditor.Rendering.Universal;

using static Enums;

namespace Server.Game
{
	public class NetWorld
	{
		public readonly BaseGameRule GameRule;

		public NetPhysics2D Physics2D = new();
		public GameFrameInfo InputInfo { get; set; }
		public NetCharacter[] NetCharacters { get; set; }

		private readonly WorldData _worldData;
		private Action _update;
		private Dictionary<uint, INetObject> _netObjDict = new();

		public NetWorld(WorldData data, BaseGameRule gameRule)
		{
			GameRule = gameRule;
			GameRule.World = this;
			_worldData = data;
			_update = UpdateGameLogic;
			_update += UpdatePlayers;
			uint i = 0x10;
			foreach (var netObjData in data.NetObjectDatas)
			{
				var wall = new Wall(i, this, NetObjectTag.Wall, netObjData.BoxCollider.Offset, netObjData.BoxCollider.Size)
				{
					Position = netObjData.Position,
					Rotation = netObjData.Rotation,
				};

				AddNewNetObject(i++, wall);
			}

			NetCharacters = new NetCharacter[6];
		}

		public void Update() => _update();

		public void UpdateGameLogic()
		{
			GameRule.UpdateGameLogic();
		}

		public void UpdatePlayers()
		{
			foreach (var player in NetCharacters)
			{
				if (player is null || player.Active is false)
				{
					continue;
				}

				Loggers.Game.Information("Player [{0}]", player.ObjectId);
				player.UpdateInput(InputInfo.Inputs[player.ObjectId]);
				player.Update();
				Loggers.Game.Information("Position [{0:x},{1:x},{2:x}]] : ", player.Position.x.RawValue, player.Position.y.RawValue, player.Position.z.RawValue);
			}
		}

		public NetCharacter AddNewCharacter(int inGameId, CharacterType type)
		{
			var id = (uint)inGameId;
			//var character = new NetCharacterKnight(id, GameRule.GetTeamType(id), _worldData.SpawnPoints[inGameId], sQuaternion.identity, this);
			var character = new NCharacterShelly(id, GameRule.GetTeamType(id), _worldData.SpawnPoints[inGameId], sQuaternion.identity, this);
			NetCharacters[inGameId] = character;
			_netObjDict.Add(id, character);
			return character;
		}

		public void AddNewNetObject(uint inGameId, INetObject obj)
		{
			if (obj is NetCharacter)
			{
				NetCharacters[inGameId] = (obj as NetCharacter);
				_netObjDict.Add(inGameId, obj);
			}
			else if (obj is INetUpdatable)
			{
				_update += (obj as INetUpdatable).Update;
			}

			if (obj is INetCollidable2D)
			{
				Physics2D.RegisterCollider((obj as INetCollidable2D).Collider);
			}

			_netObjDict.Add(inGameId, obj);
		}

		public INetObject FindNetObject(uint inGameId) => _netObjDict[inGameId];

		public void FindNetObjects(Func<INetObject, bool> condition, IList<INetObject> result)
		{
			foreach (var obj in _netObjDict.Values)
			{
				if (condition(obj))
				{
					result.Add(obj);
				}
			}
		}

		public bool FindAllAndBroadcast(Func<INetObject, bool> condition, Action<INetObject> action)
		{
			var count = 0;
			foreach (var obj in _netObjDict.Values)
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

