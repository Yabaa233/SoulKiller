using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/DashBoomEmyStateData")]
public class DBoomEmyStateData_SO : ScriptableObject
{
    [Header("攻撃距離")]
    public float attackDistance;
    [Header("攻撃間隔")]
    public float attackSpeed;
    [Header("基本移動速度")]
    public float moveSpeed;
    [Header("スプリント力")]
    public float dashPower;
    [Header("自爆遅延")]
    public float boomDelay;
    [Header("自爆音効")]
    public FMODUnity.EventReference boomEffect;
}
