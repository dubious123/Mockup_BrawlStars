using UnityEngine;

public class Temp : MonoBehaviour
{
	[SerializeField] private GameObject _prefab;
	[SerializeField] private GameObject _prefab2;
	[SerializeField] private Transform _outer;
	[SerializeField] private Transform _inner;

	public void Run()
	{
		for (int i = 0; i <= 90; i += 2)
		{
			var outerGo = Instantiate(_prefab, _outer);
			outerGo.transform.localPosition = new Vector3(-Mathf.Cos(i * Mathf.Deg2Rad) * 1.7f, 0.02f, -Mathf.Sin(i * Mathf.Deg2Rad) * 1.7f);
			outerGo.transform.localEulerAngles = new Vector3(0, -i, 0);
			var innerGo = Instantiate(_prefab, _inner);
			innerGo.transform.localPosition = new Vector3(-Mathf.Cos(i * Mathf.Deg2Rad) * 1.3f, 0.02f, -Mathf.Sin(i * Mathf.Deg2Rad) * 1.3f);
			innerGo.transform.localEulerAngles = new Vector3(0, -i, 0);
		}
	}

	public void Run2()
	{
		var interval = (90 - 6.37937021f * 2) / 6;
		for (float i = 6.37937021f; i <= 90; i += interval)
		{
			var go = Instantiate(_prefab2, transform);
			go.transform.localPosition = new Vector3(-Mathf.Cos(i * Mathf.Deg2Rad) * 1.5f, 0.016f, -Mathf.Sin(i * Mathf.Deg2Rad) * 1.5f);
			go.transform.localEulerAngles = new Vector3(0, -i, 0);
		}
	}
}
