using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/DashBoomEmyStateData")]
public class DBoomEmyStateData_SO : ScriptableObject
{
    [Header("攻击距离")]
    public float attackDistance;
    [Header("攻击间隔")]
    public float attackSpeed;
    [Header("基础移动速度")]
    public float moveSpeed;
    [Header("冲刺力度")]
    public float dashPower;
    [Header("自爆延迟")]
    public float boomDelay;
    [Header("自爆音效")]
    public FMODUnity.EventReference boomEffect;
}
