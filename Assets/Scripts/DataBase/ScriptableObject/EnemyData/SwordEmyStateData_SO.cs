using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/SwordEmyStateData")]
public class SwordEmyStateData_SO : ScriptableObject
{
    [Header("攻撃距離")]
    public float attackDistance;
    [Header("攻撃間隔")]
    public float attackSpeed;
    [Header("基本移動速度")]
    public float moveSpeed;
    [Header("トリプルヒット音響効果")]
    public FMODUnity.EventReference swordEffect;
}
