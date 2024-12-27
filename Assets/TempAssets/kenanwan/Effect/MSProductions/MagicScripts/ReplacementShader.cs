using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ReplacementShader : MonoBehaviour
{
    public Shader replacementShader;

    public void OnEnable()
    {
        if (replacementShader != null)
            GetComponent<Camera>().SetReplacementShader(replacementShader, "RenderType");
    }

    public void OnDisable()
    {
        GetComponent<Camera>().ResetReplacementShader();
    }
}
