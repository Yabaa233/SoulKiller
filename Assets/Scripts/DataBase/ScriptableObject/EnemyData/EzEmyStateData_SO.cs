using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObject/Battle/EzEmyStateData")]
///<summary>

////// 第一種類の敵の攻撃状態を記述するファイル

///</summary>
[System.Serializable]
public class EzEmyStateData_SO : ScriptableObject
{
   [Header("攻撃距離")]
   public float attackDistance;
   [Header("攻撃速度")]
   public float attackSpeed;
   [Header("基本移動速度")]
   public float moveSpeed;
   [Header("衝突の強度")]
   public float dashPower;
    [Header("衝突音効")]
    public FMODUnity.EventReference dashEffect;
}
