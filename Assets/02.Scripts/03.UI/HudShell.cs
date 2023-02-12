using Server.Game;

using UnityEngine;

public class HudShell : MonoBehaviour
{
	[SerializeField] private GameObject _shellPrefab;
	[SerializeField] private ClientCharacter _cPlayer;

	private NetBasicAttack _netBasicAttack;
	private ShellHelper[] _shells;
	private int _beforeShellCount;

	public void Init()
	{
		_netBasicAttack = _cPlayer.NPlayer.BasicAttack;
		_beforeShellCount = _netBasicAttack.MaxShellCount;
		if (_netBasicAttack.MaxShellCount <= 0 || _netBasicAttack.MaxShellCount > 100)
		{
			throw new System.Exception();
		}

		if (User.TeamId != _netBasicAttack.Character.TeamId)
		{
			Destroy(gameObject);
			return;
		}

		_shells = new ShellHelper[_netBasicAttack.MaxShellCount];
		float length = Mathf.Floor(100f / _netBasicAttack.MaxShellCount) / 100f;
		float interval = _netBasicAttack.MaxShellCount == 1 ? 0 : (1f - length * _netBasicAttack.MaxShellCount) / (_netBasicAttack.MaxShellCount - 1);
		for (int i = 0; i < _netBasicAttack.MaxShellCount; ++i)
		{
			var rect = Instantiate(_shellPrefab, transform).GetComponent<RectTransform>();
			var minX = i * (length + interval);
			var maxX = Mathf.Min(minX + length, 1f);
			rect.SetAnchorX(new Vector2(minX, maxX));
			_shells[i] = new(rect.transform);
		}
	}

	public void HandleOneFrame()
	{
		if (_beforeShellCount == _netBasicAttack.CurrentShellCount) //0,1,2,3
		{
			if (_beforeShellCount != _netBasicAttack.MaxShellCount)
			{
				_shells[_netBasicAttack.CurrentShellCount].UpdateFillRect(_netBasicAttack.CurrentReloadDeltaFrame / (float)_netBasicAttack.ReloadFrame);
			}
		}
		else if (_beforeShellCount > _netBasicAttack.CurrentShellCount) //1, 2, 3  == Is Attack true
		{
			if (_beforeShellCount < _netBasicAttack.MaxShellCount)
			{
				_shells[_beforeShellCount].UpdateFillRect(0); //1, 2, 3 
			}

			_shells[_netBasicAttack.CurrentShellCount].UpdateFillRect(_netBasicAttack.CurrentReloadDeltaFrame / (float)_netBasicAttack.ReloadFrame); //0, 1, 2
		}
		else //0,1,2 == Is Reload
		{
			_shells[_beforeShellCount].UpdateFillRect(1f);
		}

		_beforeShellCount = _netBasicAttack.CurrentShellCount;
	}

	public void Reset()
	{
		_beforeShellCount = _netBasicAttack.CurrentShellCount;
		foreach (var shell in _shells)
		{
			shell.Reset();
		}
	}

	private class ShellHelper
	{
		private RectTransform _followRect;
		private RectTransform _fillRect;
		private float _fillPercent = 1f;
		public ShellHelper(Transform shellTransform)
		{
			_followRect = shellTransform.transform.FirstChildOrDefault(t => t.name == "Follow").GetComponent<RectTransform>();
			_fillRect = shellTransform.FirstChildOrDefault(t => t.name == "Fill").GetComponent<RectTransform>();
#if UNITY_EDITOR
			if (_followRect == null || _fillRect == null)
			{
				throw new System.Exception();
			}
#endif
		}

		public void Reset()
		{
			_followRect.anchorMax = new(1, 1);
			_followRect.gameObject.SetActive(false);
			_fillRect.gameObject.SetActive(true);
		}

		public void UpdateFillRect(float percent)
		{
			if (_fillPercent == percent)
			{
				return;
			}
			else if (percent == 1f)
			{
				Reset();
				return;
			}
			else if (percent == 0f)
			{
				_followRect.anchorMax = new(0, 1);
				_followRect.gameObject.SetActive(false);
				_fillRect.gameObject.SetActive(false);
			}

			_followRect.anchorMax = new(percent, 1);
			_followRect.gameObject.SetActive(true);
			_fillRect.gameObject.SetActive(false);
			_fillPercent = percent;
		}
	}
}
