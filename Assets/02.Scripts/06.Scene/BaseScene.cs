using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class BaseScene : MonoBehaviour
{
	public abstract Task A_Init(object param);
}
