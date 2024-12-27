using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingParticleController : MonoBehaviour
{
	public Quaternion Quaternion;
    public Transform affector;
    public float UVRangeX;
    public float UVRangeY;
    private ParticleSystemRenderer psr;
	
	void Start ()
	{
		Quaternion = transform.rotation;
		// Debug.Log(Quaternion);
        psr = GetComponent<ParticleSystemRenderer>();
        Vector4 PositionRange = new Vector4(transform.position.x - UVRangeX,transform.position.z - UVRangeY
	        ,transform.position.x + UVRangeX,transform.position.z + UVRangeY);
        // Vector3 Temxz = Quaternion * new Vector3(PositionRange.x,0,PositionRange.z);
        // Vector3 Temyw = Quaternion * new Vector3(PositionRange.y, 0, PositionRange.w);
        // Debug.Log(Temxz);
        // Debug.Log(Temyw);
        // PositionRange = new Vector4(Temxz.x, Temyw.x, Temxz.z, Temyw.z);
         Debug.Log(PositionRange);
        psr.material.SetVector("_PositionRange", PositionRange);
	}
	
	void Update () {
        psr.material.SetVector("_Affector", affector.position);
    }
}
