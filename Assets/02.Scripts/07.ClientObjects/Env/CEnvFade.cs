using MEC;
using System.Collections.Generic;
using UnityEngine;

public class CEnvFade : MonoBehaviour
{
    [SerializeField] private Renderer[] _renderers;
    [SerializeField] private float _fadeTime, _alpha;

    private string propName = "_BaseColor";

    public void FadeIn()
    {
        Timing.RunCoroutine(InternalFadeIn());
    }

    public void FadeOut()
    {
        Timing.RunCoroutine(InternalFadeOut());
    }

    private IEnumerator<float> InternalFadeIn()
    {
        for (float delta = 0; delta < _fadeTime; delta += Time.deltaTime)
        {
            SetAlpha(Mathf.Lerp(1, _alpha, delta / _fadeTime));
            yield return 0f;
        }

        yield break;
    }

    private IEnumerator<float> InternalFadeOut()
    {
        for (float delta = 0; delta < _fadeTime; delta += Time.deltaTime)
        {
            SetAlpha(Mathf.Lerp(_alpha, 1, delta / _fadeTime));
            yield return 0f;
        }

        yield break;
    }

    private void SetAlpha(float alpha)
    {
        foreach (var renderer in _renderers)
        {
            var color = renderer.material.GetColor(propName);
            color.a = alpha;
            renderer.material.SetColor(propName, color);
        }
    }
}
