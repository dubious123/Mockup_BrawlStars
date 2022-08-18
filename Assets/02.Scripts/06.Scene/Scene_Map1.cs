using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene_Map1 : BaseScene
{
	public override void Init(object param)
	{
		Debug.Log((Enums.CharacterType)param);
	}
}
