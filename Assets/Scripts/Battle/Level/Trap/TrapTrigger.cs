using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    public Action openTarp;
    private void OnTriggerEnter(Collider other)
    {
        // デバッグログ("エントリ");
        openTarp();
    }
}
