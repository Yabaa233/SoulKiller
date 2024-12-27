using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/DashEmyStateData")]
/// <summary>
/// 冲刺类敌人的攻击描述文件
/// </summary>
public class DashEmyStateData_SO : ScriptableObject
{
    [Header("攻击距离")]
    public float attackDistance;
    [Header("到达该距离就不在追玩家")]
    public float chaseDistance;
    [Header("攻击间隔")]
    public float attackSpeed;
    [Header("基础移动速度")]
    public float moveSpeed;
    [Header("冲刺力度")]
    public float dashPower;//冲刺力度
    [Header("撞击音效")]
    public FMODUnity.EventReference dashedEffect;
    
}
