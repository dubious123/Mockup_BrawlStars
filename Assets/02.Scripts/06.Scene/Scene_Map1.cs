using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Scene_Map1 : BaseScene
{
	#region SerializeField
	[SerializeField] AssetReference _dog;
	#endregion
	public override void Init(object param)
	{
		_dog.LoadAssetAsync<GameObject>().Completed += _ => Instantiate(_dog.Asset);
	}
}
