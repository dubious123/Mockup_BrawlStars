
using UnityEngine;

[ExecuteInEditMode]
public class CBoxCollider2DGizmoRenderer : CWall
{
	public Vector2 Offset;
	public Vector2 Size;
	public Color _gizmosColor;
	public bool _enableGizmo;

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		if (_enableGizmo)
		{
			Gizmos.color = _gizmosColor;
			Gizmos.DrawCube(transform.position + new Vector3(Offset.x, 0, Offset.y), new Vector3(Size.x, 1, Size.y));
		}
	}
#endif
}


