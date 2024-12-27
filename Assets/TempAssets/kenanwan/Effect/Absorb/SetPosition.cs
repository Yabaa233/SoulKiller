using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPosition : MonoBehaviour
{
    public Transform targetTransform;
    public Material[] materials;
    [Range(0 , 300)] public float radius;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Material mat in materials){
            mat.SetFloat("_Radius" , radius);
        }

        SetMatPosition();
    }
    [ContextMenu("SetMatPosition")]
    public void SetMatPosition()
    {
        foreach(Material mat in materials)
        {
            mat.SetVector("_TargetPosition", targetTransform.transform.position);
            Debug.Log(targetTransform.transform.position);
        }
    }
}
