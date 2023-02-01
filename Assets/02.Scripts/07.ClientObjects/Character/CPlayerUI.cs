using UnityEngine;

public class CPlayerUI : MonoBehaviour
{
	[SerializeField] private RectTransform _hudRect;
	[SerializeField] private RectTransform _hudCanvasRect;
	[SerializeField] private Transform _hudAnchorTransform;
	[SerializeField] private Vector3 _hudOffset;
	[SerializeField] private HudHP _hudHp;
	[SerializeField] private HudShell _hudShell;
	[SerializeField] private Sprite _selectCircleSelf;
	[SerializeField] private Sprite _selectCircleRed;
	[SerializeField] private Sprite _selectCircleBlue;
	[SerializeField] private SpriteRenderer _selectCircle;
	[SerializeField] private MoveCircle _moveCircle;
	[SerializeField] private HudPowerCircle _powerCircle;

	public void Init(CPlayer character)
	{
		if (character.TeamId == User.TeamId)
		{
			_selectCircle.sprite = _selectCircleSelf;
			_moveCircle.gameObject.SetActive(true);
			_hudCanvasRect.GetComponent<Canvas>().sortingOrder = 1;
		}
		else
		{
			_selectCircle.sprite = character.Team == User.Team ? _selectCircleBlue : _selectCircleRed;
		}

		_hudHp.Init();
		_hudShell.Init();
	}

	public void HandleOneFrame()
	{
		_hudHp.HandleOneFrame();
		if (_hudShell != null)
		{
			_hudShell.HandleOneFrame();
		}
	}

	public void Reset()
	{
		_hudHp.Reset();
		_hudShell.Reset();
	}

	private void LateUpdate()
	{
		_hudRect.anchoredPosition = Camera.main.WorldtoCanvasRectPos(_hudCanvasRect.sizeDelta, _hudAnchorTransform.position + _hudOffset);
	}
}
