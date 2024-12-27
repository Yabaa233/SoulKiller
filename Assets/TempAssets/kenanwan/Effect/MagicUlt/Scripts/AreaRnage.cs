using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//自定义粒子类
public class ParticleObject
{
    public ParticleSystem PS;
    public int ParticlesCount;
    public ParticleSystem.Particle[] Particles;
    public ParticleSystem.MainModule MainModule;

    //构造函数
    public ParticleObject(ParticleSystem PS,int ParticlesCount
        ,ParticleSystem.MainModule MainModule)
    {
        this.PS = PS;
        this.ParticlesCount = ParticlesCount;
        this.Particles = new ParticleSystem.Particle[ParticlesCount];
        this.MainModule = MainModule;
    }
    
}

public class AreaRnage : MonoBehaviour
{
    [Range(0,10)]public float ParticleSize = 1f;

    private ParticleObject mainParticle;
    private ParticleSystem.Particle[] particles;
    ParticleSystem ps;
    ParticleSystem.MainModule mainModule;

    private float originSize;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        mainModule = ps.main;
        //mainParticle = new ParticleObject(ps,ps.GetParticles(particles), ps.main);
    }

    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        originSize = mainModule.startSize.constant;

        Debug.Log(mainModule.startSize.constant + " 222");
        Debug.Log(originSize + " 111");
        
        var maxParticles = mainModule.maxParticles;
        if (particles == null || particles.Length < maxParticles)
        {
            particles = new ParticleSystem.Particle[maxParticles];
        }
        
        int particleCount = ps.GetParticles(particles);
       
        Debug.Log(particleCount);
        
        for (int i = 0; i < particleCount; i++)
        {
            particles[i].startSize = ParticleSize * originSize;
        }

        ps.SetParticles(particles, particleCount);
    }
    
    
}
