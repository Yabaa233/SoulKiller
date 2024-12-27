using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New CharacterData", menuName = "ScriptableObject/CharaterData/CharacterData")]
public class CharacterData_SO : ScriptableObject
{
    [Header("基本生命值")]
    public float baseHealth;
    [Header("基础攻击力")]
    public float baseAttack;
    [Header("基础防御力")]
    public float baseDefend;
    [Header("基础暴击率")]
    public float baseCritical;
    [Header("基础暴击伤害")]
    public float baseCriticalDamage;
    [Header("基础暴击抗性")]
    public float baseCriticalDefend;
    [Header("受击时对攻击方追加的卡肉时间 同时自己受击并卡顿这个时间")]
    public float baseStopTime;
    [Header("受击音效")]
    [Tooltip("受击音效")] public FMODUnity.EventReference getHitSound;  //受击音效
    [Header("被暴击音效")]
    [Tooltip("被暴击音效")] public FMODUnity.EventReference getCriticalSound; //受到暴击音效


    public CharacterData_SO(CharacterData_SO tempCharaterData)
    {
        this.baseHealth = tempCharaterData.baseHealth;
        this.baseAttack = tempCharaterData.baseAttack;
        this.baseDefend = tempCharaterData.baseDefend;
        this.baseCriticalDamage = tempCharaterData.baseCriticalDamage;
        this.baseCriticalDefend = tempCharaterData.baseCriticalDefend;
        this.baseStopTime = tempCharaterData.baseStopTime;
        this.getHitSound = tempCharaterData.getHitSound;
        this.getCriticalSound = tempCharaterData.getCriticalSound;

    }
}
