using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/SwordEmyStateData")]
public class SwordEmyStateData_SO : ScriptableObject
{
    [Header("攻击距离")]
    public float attackDistance;
    [Header("攻击间隔")]
    public float attackSpeed;
    [Header("基础移动速度")]
    public float moveSpeed;
    [Header("三连击音效")]
    public FMODUnity.EventReference swordEffect;
}
