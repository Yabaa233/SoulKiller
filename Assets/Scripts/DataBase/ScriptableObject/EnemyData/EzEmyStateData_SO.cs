using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObject/Battle/EzEmyStateData")]
/// <summary>
/// 第一类敌人的攻击状态描述文件
/// </summary>
[System.Serializable]
public class EzEmyStateData_SO : ScriptableObject
{
   [Header("攻击距离")]
   public float attackDistance;
   [Header("攻击速度")]
   public float attackSpeed;
   [Header("基础移动速度")]
   public float moveSpeed;
   [Header("冲撞力度")]
   public float dashPower;
    [Header("冲撞音效")]
    public FMODUnity.EventReference dashEffect;
}
