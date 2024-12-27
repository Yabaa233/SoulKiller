using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    int w, h;
    float deltaTime = 0.0f;
    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }
    private void OnGUI()
    {
        w = Screen.width;
        h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperRight;
        style.fontSize = h / 50 * 2;
        style.normal.textColor = Color.red;

        float msec = deltaTime * 1000;
        float fps = 1.0f / Time.deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);

        GUI.Label(rect, text, style);
    }
}
