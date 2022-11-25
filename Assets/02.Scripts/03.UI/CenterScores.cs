using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Unity.VectorGraphics;

using UnityEngine;

[ExecuteInEditMode]
public class CenterScores : MonoBehaviour
{
    [SerializeField] private SVGImage[] _scores;
    [SerializeField] private Sprite _imageScoreWhite;
    [SerializeField] private Sprite _imageScoreGray;
    [SerializeField] private Sprite _imageScoreBlue;
    [SerializeField] private Sprite _imageScoreRed;
    [SerializeField] private float _scoreBigScale;

    [Header("Debug")]
    [SerializeField] private int _currentScore;

    private bool _performing;
    private IEnumerator<int> _coHandle;
    private SVGImage _currentScoreUI;

    private void Start()
    {
        Reset();
    }

    private void EditorUpdate()
    {
        if (_performing)
        {
            _coHandle.MoveNext();
        }
        Debug.Log(Time.deltaTime);
    }

    public void OnRoundStart()
    {
        _currentScoreUI = _scores[_currentScore];
        _currentScoreUI.sprite = _imageScoreWhite;
        _currentScoreUI.rectTransform.localScale = new Vector3(_scoreBigScale, _scoreBigScale, _scoreBigScale);

    }

    public void OnBlueWin()
    {
        _scores[_currentScore].sprite = _imageScoreBlue;
        InternalHandleWin();
        Debug.Log("Blue");
    }

    public void OnRedWin()
    {
        _scores[_currentScore].sprite = _imageScoreRed;
        InternalHandleWin();
        Debug.Log("Red");
    }

    public void Reset()
    {
        _performing = false;
        _currentScore = 0;
        foreach (var score in _scores)
        {
            score.sprite = _imageScoreGray;
            score.rectTransform.localScale = Vector3.one;
        }
        Debug.Log("Reset");
    }

    private void InternalHandleWin()
    {
        _currentScoreUI = _scores[_currentScore++];
        _coHandle = CoShrink();
        UnityEditor.EditorApplication.update -= EditorUpdate;
        UnityEditor.EditorApplication.update += EditorUpdate;
        _performing = true;
    }

    private void MoveNext()
    {
        ++_currentScore;
    }

    private IEnumerator<int> CoShrink()
    {
        var targetScale = Vector3.one;
        var startScale = new Vector3(_scoreBigScale, _scoreBigScale, _scoreBigScale);
        for (float delta = 0; delta < 1; delta += Time.deltaTime * 4)
        {
            _currentScoreUI.rectTransform.localScale = Vector3.Lerp(startScale, targetScale, (delta) * (2 - delta));
            yield return 0;
        }

        _currentScoreUI.rectTransform.localScale = targetScale;
        _performing = false;

        UnityEditor.EditorApplication.update -= EditorUpdate;
        yield break;
    }
}
