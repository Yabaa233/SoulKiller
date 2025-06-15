using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New CharacterData", menuName = "ScriptableObject/CharaterData/CharacterData")]
public class CharacterData_SO : ScriptableObject
{
    [Header("基本生命値")]
    public float baseHealth;
    [Header("基本攻撃力")]
    public float baseAttack;
    [Header("基本防御力")]
    public float baseDefend;
    [Header("基本クリティカル率")]
    public float baseCritical;
    [Header("基本クリティカルダメージ")]
    public float baseCriticalDamage;
    [Header("基本クリティカル抵抗力")]
    public float baseCriticalDefend;
    [Header("攻撃を受けた際に攻撃者に追加されるカードの時間、同時に自分も攻撃を受けてカードが停止する時間")]
    public float baseStopTime;
    [Header("被打撃音効")]
    [Tooltip("被打撃音効")] public FMODUnity.EventReference getHitSound;  //被打撃音効
    [Header("クリティカルヒットの音効果")]
    [Tooltip("クリティカルヒットの音効果")] public FMODUnity.EventReference getCriticalSound; //クリティカルヒット音效


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
