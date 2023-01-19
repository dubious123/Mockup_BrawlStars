using System.IO;
using System.Linq;

using Server.Game;
using Server.Game.Data;

using Unity.VisualScripting;

using UnityEngine;

using static Enums;

public class WorldDataHelper : MonoBehaviour
{
	[Header("Create World")]
	[SerializeField] private Transform[] _spawnPoints;

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
		var envs = transform.GetComponentsInChildren<CEnv>(true);

		var data = new WorldData
		{
			NetObjectDatas = new NetObjectData[envs.Length],
			SpawnPoints = _spawnPoints.Select(t => (sVector3)t.position).ToArray()
		};

		for (int i = 0; i < envs.Length; i++)
		{
			var env = envs[i];
			if (env is CWall wall)
			{
				var size = wall is not CBoxCollider2DGizmoRenderer renderer ? sVector2.one : (sVector2)renderer.Size;
				data.NetObjectDatas[i] = new NetObjectData()
				{
					NetObjectId = (uint)NetObjectType.Env_Wall,
					Position = (sVector3)wall.transform.position,
					Rotation = sQuaternion.identity,
					BoxCollider = new NetBoxCollider2DData()
					{
						Size = size,
						Offset = sVector2.zero,
					}
				};
			}
			else if (env is CTree tree)
			{
				data.NetObjectDatas[i] = new NetObjectData()
				{
					NetObjectId = (uint)NetObjectType.Env_Tree,
					Position = (sVector3)tree.transform.position,
					Rotation = sQuaternion.identity,
					BoxCollider = new NetBoxCollider2DData()
					{
						Size = sVector2.one,
						Offset = sVector2.zero,
					}
				};
			}
		}

		Debug.Log("Create world data Complete");

		return data;
	}
}
