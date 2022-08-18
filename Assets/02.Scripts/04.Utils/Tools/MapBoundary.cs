#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Tilemaps;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MapBoundary : MonoBehaviour
{
	[SerializeField] int _xMin;
	[SerializeField] int _xMax;
	[SerializeField] int _yMin;
	[SerializeField] int _yMax;
	[SerializeField] GameObject _tile;

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

}
#endif
