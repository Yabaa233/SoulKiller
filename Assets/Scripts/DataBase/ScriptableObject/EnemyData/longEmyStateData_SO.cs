using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// [CreateAssetMenu(menuName = "ScriptableObject/Battle/GunEmyStateData")]
/// <summary>
/// 远距离攻击敌人的描述文件，所有属性都放在这里
/// </summary>
public class longEmyStateData_SO : ScriptableObject
{
    [Header("攻击距离")]
    public float attackDistance;
    [Header("攻击间隔")]
    public float attackSpeed;
    [Header("基础移动速度")]
    public float moveSpeed;
    [Header("攻击音效")]
    public FMODUnity.EventReference attackSound;
    [Header("闪现音效")]

    public FMODUnity.EventReference shiftSound;

}
