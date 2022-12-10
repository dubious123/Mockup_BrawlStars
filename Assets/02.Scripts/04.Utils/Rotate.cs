using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Rotate : MonoBehaviour
{
	private void OnEnable()
	{
		transform.rotation = Quaternion.identity;
	}

	private void Update()
	{
		var rot = transform.rotation.eulerAngles;
		rot.z += 5;
		transform.rotation = Quaternion.Euler(rot);
	}
}
