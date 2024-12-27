using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class HighLight : MonoBehaviour
{
    public AnimationCurve LightIntensity;
    public float Intensity = 1f;
    [SerializeField] private Light pointLight; 
    [SerializeField] private float timer; 
    // Start is called before the first frame update
    void Start()
    {
        pointLight = GetComponentInChildren<Light>();
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 1)
        {
            timer = 0;
        }

        pointLight.intensity = LightIntensity.Evaluate(timer) * Intensity;
    }
}
