using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CEnvFade))]
public class CEnvFadeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var fade = (CEnvFade)target;

        if (GUILayout.Button("FadeIn"))
        {
            fade.FadeIn();
        }

        else if (GUILayout.Button("FadeOut"))
        {
            fade.FadeOut();
        }
    }
}
