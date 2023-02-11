using UnityEngine;

public class UIRuntimeCellSizeVerticle : MonoBehaviour
{
	[SerializeField] private int _columnCount;
	[SerializeField] private float _cellAspectRatio;
	[SerializeField] private Vector2 _cellSpacing;

	public void Init()
	{
		var cellWidth = (1f - _cellSpacing.x * (_columnCount - 1)) / _columnCount;
		var cellHeight = cellWidth * _cellAspectRatio;
		var rectTrans = transform.GetComponent<RectTransform>();
		for (var i = 0; i < transform.childCount; ++i)
		{
			var childRectTrans = transform.GetChild(i).GetComponent<RectTransform>();
			var anchorMinX = (cellWidth + _cellSpacing.x) * (i % 3);
			var anchorMaxY = rectTrans.anchorMax.y - (i / 3) * (cellHeight + _cellSpacing.y);
			childRectTrans.anchorMax = new Vector2(anchorMinX + cellWidth, anchorMaxY);
			childRectTrans.anchorMin = new Vector2(anchorMinX, anchorMaxY - cellHeight);
		}
	}
}