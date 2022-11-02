using System.IO;
using System.Linq;

using Server.Game.Data;

using UnityEngine;

public class WorldDataHelper : MonoBehaviour
{
#if UNITY_EDITOR
	[Header("Create World")]
	[SerializeField] private string _filePath;
	[SerializeField] private Transform[] _spawnPoints;
	[SerializeField] private CBoxCollider2DGizmoRenderer[] _walls;

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
		var data = new WorldData
		{
			NetObjectDatas = new NetObjectData[_walls.Length],
			SpawnPoints = _spawnPoints.Select(t => (sVector3)t.position).ToArray()
		};

		for (int i = 0; i < _walls.Length; i++)
		{
			data.NetObjectDatas[i] = new NetObjectData()
			{
				NetObjectId = 1,
				Position = (sVector3)_walls[i].transform.position,
				Rotation = sQuaternion.identity,
				BoxCollider = new NetBoxCollider2DData()
				{
					Size = (sVector2)_walls[i].Size,
					Offset = (sVector2)_walls[i].Offset,
				}
			};
		}

		return data;
	}
}
