using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSprite : MonoBehaviour
{

    public Material enemyMaterial;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            enemyMaterial.SetFloat("_TimeStamp", Time.timeSinceLevelLoad);
        }
    }
}
