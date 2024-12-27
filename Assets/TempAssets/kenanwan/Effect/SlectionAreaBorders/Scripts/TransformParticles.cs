using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TransformParticles : MonoBehaviour
{
    public ParticleSystem[] ParticleSystems;
    public Vector3 ParticleScale = Vector3.one;


    private void Awake()
    {
        ParticleScale = new Vector3(1, 1, 1);
        ParticleSystems = gameObject.GetComponentsInChildren<ParticleSystem>();
    }

    private void Update()
    {
        for (int i = 0; i < ParticleSystems.Length; i++)
        {
            ParticleSystems[i].transform.localScale = ParticleScale;
        }
    }
}
