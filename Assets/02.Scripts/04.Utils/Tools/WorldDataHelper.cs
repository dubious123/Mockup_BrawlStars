using System.IO;
using System.Linq;

using Server.Game;
using Server.Game.Data;

using UnityEngine;

using static Enums;

public class WorldDataHelper : MonoBehaviour
{
	[Header("Create World")]
	[SerializeField] private Transform[] _spawnPoints;
	[SerializeField] private CBoxCollider2DGizmoRenderer[] _walls;
	[SerializeField] private Transform _env;

#if UNITY_EDITOR
	[SerializeField] private string _filePath;
	[ContextMenu("Create World Data")]
	public void GenerateWorldData()
	{
		Newtonsoft.Json.JsonSerializerSettings setting = new();
		setting.Formatting = Newtonsoft.Json.Formatting.Indented;
		setting.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
		setting.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
		File.WriteAllText(_filePath, Newtonsoft.Json.JsonConvert.SerializeObject(GetWorldData(), setting));
	}
#endif
	public WorldData GetWorldData()
	{
		var walls = _env.GetComponentsInChildren<CWall>(true);
		var trees = _env.GetComponentsInChildren<CTree>(true);

		var data = new WorldData
		{
			NetObjectDatas = new NetObjectData[walls.Length + _walls.Length + trees.Length],
			SpawnPoints = _spawnPoints.Select(t => (sVector3)t.position).ToArray()
		};

		for (int i = 0; i < walls.Length; i++)
		{
			data.NetObjectDatas[i] = new NetObjectData()
			{
				NetObjectId = (uint)NetObjectType.Env_Wall,
				Position = (sVector3)walls[i].transform.position,
				Rotation = sQuaternion.identity,
				BoxCollider = new NetBoxCollider2DData()
				{
					Size = sVector2.one,
					Offset = sVector2.zero,
				}
			};
		}

		for (int i = 0; i < _walls.Length; i++)
		{
			data.NetObjectDatas[i + walls.Length] = new NetObjectData()
			{
				NetObjectId = (uint)NetObjectType.Env_Wall,
				Position = (sVector3)_walls[i].transform.position,
				Rotation = sQuaternion.identity,
				BoxCollider = new NetBoxCollider2DData()
				{
					Size = (sVector2)_walls[i].Size,
					Offset = (sVector2)_walls[i].Offset,
				}
			};
		}

		for (int i = 0; i < trees.Length; i++)
		{
			data.NetObjectDatas[i + walls.Length + _walls.Length] = new NetObjectData()
			{
				NetObjectId = (uint)NetObjectType.Env_Tree,
				Position = (sVector3)trees[i].transform.position,
				Rotation = sQuaternion.identity,
				BoxCollider = new NetBoxCollider2DData()
				{
					Size = sVector2.one,
					Offset = sVector2.zero,
				}
			};
		}

		Debug.Log("Create world data Complete");

		return data;
	}
}
