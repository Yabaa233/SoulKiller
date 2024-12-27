using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[ExecuteInEditMode]
public class SoulParticlesControl : MonoBehaviour
{
    public Transform Target;
    public float Force = 1;
    public float StopDistance = 0;
    public float StartSpeed;
    public float ReturnInterval = 0.01f;
    ParticleSystem ps;
    ParticleSystem.Particle[] particles;
    ParticleSystem.MainModule mainModule;
    private float timer;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        mainModule = ps.main;
    }

    private void Update()
    {
        //粒子阶段计时
        if (ps.isPlaying)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
        }
        
        if (Target == null) return;
        var maxParticles = mainModule.maxParticles;
        if (particles == null || particles.Length < maxParticles)
        {
            particles = new ParticleSystem.Particle[maxParticles];
        }
        var targetTransformedPosition = Vector3.zero;
        if (mainModule.simulationSpace == ParticleSystemSimulationSpace.Local)
            targetTransformedPosition = transform.InverseTransformPoint(Target.position);
        if (mainModule.simulationSpace == ParticleSystemSimulationSpace.World)
            targetTransformedPosition = Target.position;
        
        //设置粒子初始速度
        mainModule.startSpeed = StartSpeed;
        //获取粒子个数
        int particleCount = ps.GetParticles(particles);

        //粒子返回速度
        float forceDeltaTime = Time.deltaTime * Force;
        
        for (int i = 0; i < particleCount; i++)
        {
            //粒子漂浮阶段
            if (timer > 0.3f && timer < 1f)
            {
                particles[i].velocity = new Vector3(0,1,0);
            }
            
            //粒子返回方向
            var distanceToParticle = targetTransformedPosition - particles[i].position;
            //粒子返回阶段
            if (timer > 1f  + ReturnInterval * i)
            {
                var directionToTarget = Vector3.Normalize(distanceToParticle);
                var seekForce = directionToTarget*forceDeltaTime;
                particles[i].velocity += seekForce;
            }
            
            //粒子停止在角色身边
            if (timer > 1f && StopDistance > 0.001f && distanceToParticle.magnitude < StopDistance)
            {
                particles[i].velocity = Vector3.zero;
            }
        }
        ps.SetParticles(particles, particleCount);
    }
}
