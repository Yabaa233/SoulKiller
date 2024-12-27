﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleInfo
{
    public float radius = 0;
    public float angle = 0;
    public ParticleInfo(float radius, float angle)
    {
        this.radius = radius;   // 半径  
        this.angle = angle;     // 角度  
    }
}

public class StarRing : MonoBehaviour {
    private ParticleSystem particleSys;  // 粒子系统  
    private ParticleSystem.Particle[] particleArr;  // 粒子数组  
    private ParticleInfo[] info; // 粒子信息数组  
    private ParticleSystem.TrailModule particleSysTrails;
    
    public float MinRadius = 6f;
    public float MaxRadius = 10f;
    public float MinSize = 6f;
    public float MaxSize = 10f;
    public float MinHeight = -10f;
    public float MaxHeight = 10f;
    public float speed = 0.25f;            // 速度    
    public int count = 500;       // 粒子数量 
    void Start () {
        // 初始化粒子数组  
        particleArr = new ParticleSystem.Particle[count];
        info = new ParticleInfo[count];
        
        // 初始化粒子系统  
        particleSys = this.GetComponent<ParticleSystem>();
        particleSysTrails = particleSys.trails;
        particleSysTrails.enabled = false;
        particleSysTrails.dieWithParticles = false;
        //particleSys.loop = false;  //不能取消粒子循环，因为粒子尾迹会继承粒子的生命周期
                    //粒子的尾迹会在前面的粒子生命周期结束之后被摧毁，开启循环就不会被摧毁 
        particleSys.startSpeed = 0;            // 设置粒子初速度      
        particleSys.maxParticles = count;      // 设置最大粒子量  
        particleSys.Emit(count);               // 发射粒子  
        particleSys.GetParticles(particleArr);

        IniAll();   // 初始化所有粒子
    }

    void Update()
    {
        
        if (!particleSysTrails.enabled)
        {
            particleSysTrails.enabled = true;
        }
        
        for (int i = 0; i < count; i++)
        {
            // 除以半径是为了使速度更加多样化
            float rotateSpeed = (speed / info[i].radius) * (i % 10 + 1);
        
            // 一半粒子顺时针转，一半粒子逆时针转
            if (i % 2 == 0)
            {
                info[i].angle -= rotateSpeed;
            }
            else
            {
                info[i].angle += rotateSpeed;
            }                
            
            // 保证角度合法
            info[i].angle %= 360.0f;
            // 转换成弧度制
            float radian = info[i].angle * Mathf.PI / 180;
        
            particleArr[i].position = new Vector3(info[i].radius * Mathf.Cos(radian), 
                particleArr[i].position.y, 
                info[i].radius * Mathf.Sin(radian));
            // particleArr[i].position = new Vector3(info[i].radius , 
            //     particleArr[i].position.y, 
            //     info[i].radius );
        }
        // 通过粒子数组设置粒子系统
        particleSys.SetParticles(particleArr, particleArr.Length);
    }

    void IniAll()
    {          
        float minRadius = MinRadius;  // 最小半径  
        float maxRadius = MaxRadius; // 最大半径           
        for (int i = 0; i < count; ++i)
        {   
            // 随机每个粒子半径，集中于平均半径附近  
            float midRadius = (maxRadius + minRadius) / 2;
            float minRate = Random.Range(1.0f, midRadius / minRadius);
            float maxRate = Random.Range(midRadius / maxRadius, 1.0f);
            float radius = Random.Range(minRadius * minRate, maxRadius * maxRate);

            // 随机每个粒子的角度  
            float angle = Random.Range(0, 360);
            // 转换成弧度制
            float radian = angle / 180 * Mathf.PI;

            // 随机每个粒子的大小
            float size = Random.Range(MinSize, MaxSize);

            info[i] = new ParticleInfo(radius, angle);            
            
            particleArr[i].position = new Vector3(info[i].radius * Mathf.Cos(radian), 
                Random.Range(MinHeight,MaxHeight), info[i].radius * Mathf.Sin(radian));
            // if (particleArr[i].position.y > 0)
            // {
            //     particleArr[i].position = new Vector3(info[i].radius * Mathf.Cos(radian), 
            //         -particleArr[i].position.y, info[i].radius * Mathf.Sin(radian));
            // }
            
            particleArr[i].size = size;            
        }
        // 通过初始化好的粒子数组设置粒子系统
        particleSys.SetParticles(particleArr, particleArr.Length);
    }
}
