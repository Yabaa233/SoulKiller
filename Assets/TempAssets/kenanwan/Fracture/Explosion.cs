using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float MinForce;
    public float MaxForce;
    public float Radius;

    void Start()
    {
        ExplosionForce();
        Destroy(this.gameObject, 5.0f);
    }

    void ExplosionForce ()
    {
        foreach (Transform t in transform)
        {
            var rb = t.GetComponent<Rigidbody>();

            if(rb != null)
                rb.AddExplosionForce (Random.Range(MinForce, MaxForce), transform.position, Radius);
        }
    }
}
