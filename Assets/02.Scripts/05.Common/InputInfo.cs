using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public struct InputInfo
{
	[SerializeField] public long StartTick;//{ get; init; } //N
	[SerializeField] public long TargetTick;//{ get; init; } //N + alpha
	[SerializeField] public Vector3 MoveInput;//{ get; init; }
	[SerializeField] public Vector3 LookInput;//{ get; init; }
}
