using UnityEngine;

[ExecuteInEditMode]
public class Wall : MonoBehaviour, INetObject
{
	[SerializeField] private NetCollider2D _collider;
	[SerializeField] private Color _gizmosColor;
	[SerializeField] private bool _enableGizmo;
	[field: SerializeField] public Enums.NetObjectTag Tag { get; set; } = Enums.NetObjectTag.Wall;

	public sVector3 Position { get; set; }
	public sQuaternion Rotation { get; set; }

	public void Awake()
	{
		_collider.Init(this);
		Position = (sVector3)transform.position;
	}

	private void OnDrawGizmos()
	{
		if (_enableGizmo)
		{
			_collider.Init(this);
			Position = (sVector3)transform.position;
			Gizmos.color = _gizmosColor;
			_collider.DrawGizmo();
		}
	}
}
