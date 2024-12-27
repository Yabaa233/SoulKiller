using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AreaShrink : MonoBehaviour
{
    public float StopDistance = 1; 
    Transform Target;
    ParticleSystem ps;
    ParticleSystem.Particle[] particles;
    ParticleSystem.MainModule mainModule;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        mainModule = ps.main;
        Target = transform;
    }

    void LateUpdate()
    {
        if (Target == null) return;
        var maxParticles = mainModule.maxParticles;
        //初始化粒子
        if (particles == null || particles.Length < maxParticles)
        {
            particles = new ParticleSystem.Particle[maxParticles];
        }
        int particleCount = ps.GetParticles(particles);

        var targetTransformedPosition = Vector3.zero;
        if (mainModule.simulationSpace == ParticleSystemSimulationSpace.Local)
            targetTransformedPosition = transform.InverseTransformPoint(Target.position);
        if (mainModule.simulationSpace == ParticleSystemSimulationSpace.World)
            targetTransformedPosition = Target.position;

        for (int i = 0; i < particleCount; i++)
        {
            var distanceToParticle = targetTransformedPosition - particles[i].position;
           
            //让粒子在靠近中心的时候停止
            if (StopDistance > 0.001f && distanceToParticle.magnitude < StopDistance)
            {
                particles[i].velocity = Vector3.zero;
            }
        }
        
        //把计算好的粒子传给粒子系统
        ps.SetParticles(particles, particleCount);
    }
}
