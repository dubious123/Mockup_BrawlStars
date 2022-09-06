using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using MEC;
using System.Threading.Tasks;

public class Scene_Map1 : BaseScene
{
	#region SerializeField
	[SerializeField] AssetReference _dog;
	[SerializeField] Transform _spawnPoint;
	#endregion
	public override void Init(object param)
	{
		var handle = _dog.LoadAssetAsync<GameObject>();
		handle.Completed += _ =>
		{
			GameObject character = Instantiate(_dog.Asset as GameObject, _spawnPoint.position, Quaternion.identity);
			Camera.main.GetComponent<GameCameraController>().FollowTarget = character.transform;
		};
	}
}
