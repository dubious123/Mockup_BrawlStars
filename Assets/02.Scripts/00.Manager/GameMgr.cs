using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
	#region Serialize Fields
	[SerializeField] Transform _spawningPoint_red1;
	[SerializeField] Transform _spawningPoint_red2;
	[SerializeField] Transform _spawningPoint_blue1;
	[SerializeField] Transform _spawningPoint_blue2;

	[SerializeField] GameObject _red1;
	[SerializeField] GameObject _blue1;
	#endregion


	void Start()
	{
		_red1.transform.position = _spawningPoint_red1.position;
		_blue1.transform.position = _spawningPoint_blue1.position;
	}
}
