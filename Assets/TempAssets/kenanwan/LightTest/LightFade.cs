using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class LightFade : MonoBehaviour
{
    public Light point;

    private void Awake()
    {
        //LightmapperUtils.Extract(l, ref point); LightmapperUtils.Extract(l, out cookie); ld.Init(ref spot, ref cookie);
    }
}
