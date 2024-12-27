using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Combo")]
public class ComboNode : ScriptableObject
{
    [Header("基本状态")]
    [Tooltip("攻击特效")] public GameObject attackEffect;    //攻击特效
    [Tooltip("攻击基础倍率")] public float baseDamage;    //招式伤害倍率
    [Tooltip("攻击基础范围")] public Vector3 attackRange;    //招式基础范围
    [Tooltip("攻击范围的偏移")] public float attackRangeDeviation = 0;

    // [Tooltip("攻击动画强制播放时间")] public float stiffnessTime; //硬直时间
    [Tooltip("攻击动画强制播放比例")][Range(0.0f, 1.0f)] public float forceAnimProgress; //强制播放动画的比例


    [Header("音效")]
    [Tooltip("攻击音效")] public FMODUnity.EventReference attackSound;  //攻击音效
    [Tooltip("暴击音效")] public FMODUnity.EventReference criticalSound;  //暴击音效


    [Header("击退效果")]
    [Tooltip("X Z方向的击退比例")] public Vector2 forceDir;    //击退方向
    [Tooltip("击退力大小")] public float forcePower;    //击退力大小

    [Header("震动效果")]
    [Tooltip("震动类型")] public shake_type type;
    [Tooltip("震动持续时间")] public float shake_time;
    [Tooltip("震动振幅")] public float amp = 1;
    [Tooltip("震动频率")] public float fre = 1;


    [Header("特殊运动状态")]
    [Tooltip("补偿速度")] public float compensateSpeed;   //补偿速度
    [Tooltip("强制位移速度")] public Vector2 forceSpeed;  //强制位移速度
    [Tooltip("自动索敌检测盒尺寸的 一半 ")] public Vector3 halfPlungeBoxSize;    //索敌盒子尺寸的一半
    [Tooltip("自动索敌追击敌人速率")] public float plungePower; //自动索敌追击敌人速率

}
