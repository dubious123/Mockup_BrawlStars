#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Newtonsoft.Json;

using Server.Game.Data;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Tilemaps;

public class World_Tool : MonoBehaviour
{
	//[Header("Map Boundary")]
	//[SerializeField] private int _xMin;
	//[SerializeField] private int _xMax;
	//[SerializeField] private int _yMin;
	//[SerializeField] private int _yMax;
	//[SerializeField] private GameObject _tile;

	//[Header("Create Map Data")]
	//[SerializeField] private string _filePath;
	//[SerializeField] private Tilemap _walls;
	//[SerializeField] private Tilemap _extras;

	[Header("Create World")]
	[SerializeField] private string _filePath;


	//public int XMin => _xMin;
	//public int XMax => _xMax;
	//public int YMin => _yMin;
	//public int YMax => _yMax;

	//[ContextMenu("Fill Tiles")]
	//private void FillBase()
	//{
	//	var children = new List<GameObject>();
	//	for (int i = 0; i < transform.childCount; i++)
	//	{
	//		children.Add(transform.GetChild(i).gameObject);
	//	}
	//	foreach (var child in children)
	//	{
	//		DestroyImmediate(child);
	//	}

	//	var map = GetComponent<Tilemap>();
	//	for (int x = -_xMin; x <= _xMax; x++)
	//	{
	//		for (int y = -_yMin; y <= _yMax; y++)
	//		{
	//			Instantiate(_tile, map.GetCellCenterLocal(new Vector3Int(x, y, 0)), Quaternion.identity, transform);
	//		}
	//	}
	//}

	//[ContextMenu("Create Map Data")]
	//private void CreateMapData()
	//{
	//	var wallSet = new HashSet<Vector2Int>();
	//	var blueSpawnSet = new HashSet<Vector2Int>();
	//	var redSpawnSet = new HashSet<Vector2Int>();
	//	for (int i = 0; i < _walls.transform.childCount; i++)
	//	{
	//		var pos = _walls.transform.GetChild(i).position;
	//		wallSet.Add(Vector2Int.CeilToInt(new Vector2(pos.x - 0.5f, pos.z - 0.5f)));
	//	}
	//	for (int i = 0; i < _extras.transform.childCount; i++)
	//	{
	//		var child = _extras.transform.GetChild(i);
	//		var set = child.name.Contains("Red") ? redSpawnSet : blueSpawnSet;
	//		var pos = child.position;
	//		set.Add(Vector2Int.CeilToInt(new Vector2(pos.x - 0.5f, pos.z - 0.5f)));
	//	}
	//	var lines = new List<string>();
	//	for (int y = -_yMin; y <= _yMax; y++)
	//	{
	//		string line = string.Empty;
	//		for (int x = -_xMin; x <= _xMax; x++)
	//		{
	//			var pos = new Vector2Int(x, y);
	//			var str =
	//				redSpawnSet.Contains(pos) ? "3" :
	//				blueSpawnSet.Contains(pos) ? "2" :
	//				wallSet.Contains(new Vector2Int(x, y)) ? "1" : "0";
	//			line += str;
	//		}
	//		Debug.Log(new string(line));
	//		lines.Add(new string(line));
	//	}
	//	File.WriteAllLines(_filePath, lines);
	//}

	[ContextMenu("Create World Data")]
	public void GenerateWorldData()
	{
		var renderers = transform.GetComponentsInChildren<CBoxCollider2DGizmoRenderer>(true);
		var worldData = new WorldData();
		worldData.NetObjectDatas = new NetObjectData[renderers.Length];

		JsonSerializerSettings setting = new JsonSerializerSettings();
		setting.Formatting = Formatting.Indented;
		setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
		for (int i = 0; i < renderers.Length; i++)
		{
			worldData.NetObjectDatas[i] = new NetObjectData()
			{
				NetObjectId = 1,
				Position = (sVector3)renderers[i].transform.position,
				Rotation = sQuaternion.identity,
				Collider = new NetBoxCollider2DData()
				{
					Size = (sVector2)renderers[i].Size,
					Offset = (sVector2)renderers[i].Offset,
				}
			};
		}
		File.WriteAllText(_filePath, JsonConvert.SerializeObject(worldData, setting));
	}

}
#endif
