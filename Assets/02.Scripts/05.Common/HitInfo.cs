using UnityEngine;

public record HitInfo
{
	public int Damage { get; init; }
	public bool IsStun { get; init; }
	public float StunDuration { get; init; }
	public float KnockbackDist { get; init; }
	public float KnockbackDuration { get; init; }
	public float KnockbackSpeed { get; init; }
	public Vector3 Pos { get; init; }
}
