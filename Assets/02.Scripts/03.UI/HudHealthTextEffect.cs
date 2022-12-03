using MEC;
using System.Collections.Generic;
using UnityEngine;

public class HudHealthTextEffect : MonoBehaviour
{
    [SerializeField] private GameObject _textPrefab;
    [SerializeField] private float _randomPosRadius;
    [SerializeField] private float _startAlpha;
    [SerializeField] private float _startSize;
    [SerializeField] private float _fadeInSize;
    [SerializeField] private float _fadeOutSize;
    [SerializeField] private float _fadeInTime;
    [SerializeField] private float _holdTime;
    [SerializeField] private float _fadeOutTime;
    [SerializeField] private Color _textColorRed;
    [SerializeField] private Color _textColorWhite;
    [SerializeField] private Vector2 _fadeInTravelDelta;
    [SerializeField] private Vector2 _fadeOutTravelDelta1;

    [field: SerializeField] public int Damage { private get; set; }
    [field: SerializeField] public bool IsSelf { private get; set; }

    public void PlayEffect()
    {
        Timing.RunCoroutine(CoPlay());
    }

    public IEnumerator<float> CoPlay()
    {
        var instance = Instantiate(_textPrefab, transform);
        var rect = instance.GetComponent<RectTransform>();
        var text = instance.GetComponent<TextWithShadow>();
        text.BodyColor = IsSelf ? _textColorWhite : _textColorRed;
        var randomStartPos = Random.insideUnitCircle * _randomPosRadius;

        rect.anchorMax = new Vector2(1 + randomStartPos.x, 1 + randomStartPos.y);
        rect.anchorMin = randomStartPos;
        text.Text = Damage.ToString();
        var startAnchorY = new Vector2(rect.anchorMin.y, rect.anchorMax.y);
        var targetAnchorY = startAnchorY + new Vector2(_fadeInTravelDelta.y, _fadeInTravelDelta.y);
        for (var delta = 0f; delta < _fadeInTime; delta += Time.deltaTime)
        {
            var t = delta / _fadeInTime;
            rect.localScale = Vector3.one * Mathf.Lerp(_startSize, _fadeInSize, t);
            text.Alpha = Mathf.Lerp(_startAlpha, 1, t * t);
            rect.SetAnchorY(Vector2.Lerp(startAnchorY, targetAnchorY, t));
            yield return 0f;
        }

        for (var delta = 0f; delta < _holdTime; delta += Time.deltaTime)
        {
            yield return 0f;
        }

        startAnchorY = new Vector2(rect.anchorMin.y, rect.anchorMax.y);
        targetAnchorY = startAnchorY + new Vector2(_fadeOutTravelDelta1.y, _fadeOutTravelDelta1.y);

        for (var delta = 0f; delta < _fadeOutTime; delta += Time.deltaTime)
        {
            var t = delta / _fadeOutTime;
            rect.localScale = Vector3.one * Mathf.Lerp(_fadeInSize, _fadeOutSize, t);
            rect.SetAnchorY(Vector2.Lerp(startAnchorY, targetAnchorY, t));
            text.Alpha = Mathf.Lerp(1, 0, t);
            yield return 0f;
        }

        Destroy(instance);
        Debug.Log("destroy");
        yield break;
    }
}
