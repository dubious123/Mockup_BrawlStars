using UnityEngine;

public class MoveCircle : MonoBehaviour
{
	[SerializeField] private CPlayer _player;
	[SerializeField] private Transform _indicatorAnchor;
	Vector2 _velocity;
	Vector2 _moveDir;

	private void Start()
	{

	}

	private void Update()
	{
		_moveDir = Vector2.SmoothDamp(_moveDir, GameInput.MoveInputAction.ReadValue<Vector2>(), ref _velocity, 0.01f, float.PositiveInfinity, Time.deltaTime);
		transform.position = _indicatorAnchor.position + new Vector3(_moveDir.x, 0, _moveDir.y);
	}
}
