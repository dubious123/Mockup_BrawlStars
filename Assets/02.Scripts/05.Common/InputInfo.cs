using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public struct InputInfo
{
	public long StartTick { get; init; } //N
	public long TargetTick { get; init; } //N + alpha
	public Vector3 MoveInput { get; init; }
	public Vector3 LookInput { get; init; }
}
