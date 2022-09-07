#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Tilemaps;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.IO;

public class Map_Tool : MonoBehaviour
{
	[Header("Map Boundary")]
	[SerializeField] int _xMin;
	[SerializeField] int _xMax;
	[SerializeField] int _yMin;
	[SerializeField] int _yMax;
	[SerializeField] GameObject _tile;

	[Header("Create Map Data")]
	[SerializeField] string _filePath;
	[SerializeField] Tilemap _walls;


	public int XMin => _xMin;
	public int XMax => _xMax;
	public int YMin => _yMin;
	public int YMax => _yMax;

	[ContextMenu("Fill Tiles")]
	void FillBase()
	{
		var children = new List<GameObject>();
		for (int i = 0; i < transform.childCount; i++)
		{
			children.Add(transform.GetChild(i).gameObject);
		}
		foreach (var child in children)
		{
			DestroyImmediate(child);
		}

		var map = GetComponent<Tilemap>();
		for (int x = -_xMin; x <= _xMax; x++)
		{
			for (int y = -_yMin; y <= _yMax; y++)
			{
				Instantiate(_tile, map.GetCellCenterLocal(new Vector3Int(x, y, 0)), Quaternion.identity, transform);
			}
		}
	}

	[ContextMenu("Create Map Data")]
	void CreateMapData()
	{
		var wallSet = new HashSet<Vector2Int>();
		for (int i = 0; i < _walls.transform.childCount; i++)
		{
			var pos = _walls.transform.GetChild(i).position;
			wallSet.Add(Vector2Int.CeilToInt(new Vector2(pos.x - 0.5f, pos.z - 0.5f)));
		}
		var lines = new List<string>();
		for (int y = -_yMin; y <= _yMax; y++)
		{
			string line = string.Empty;
			for (int x = -_xMin; x <= _xMax; x++)
			{
				var str = wallSet.Contains(new Vector2Int(x, y)) ? "1" : "0";
				line += str;
			}
			Debug.Log(new string(line));
			lines.Add(new string(line));
		}
		File.WriteAllLines(_filePath, lines);
	}

}
#endif
