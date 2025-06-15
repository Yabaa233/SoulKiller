using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/DashEmyStateData")]
/// <summary>
/// スプリント型敵の攻撃記述ファイル
/// </summary>
public class DashEmyStateData_SO : ScriptableObject
{
    [Header("攻撃距離")]
    public float attackDistance;
    [Header("その距離に到達したら、プレイヤーを追いかけない。")]
    public float chaseDistance;
    [Header("攻撃間隔")]
    public float attackSpeed;
    [Header("基本移動速度")]
    public float moveSpeed;
    [Header("スプリント力")]
    public float dashPower;//スプリント力
    [Header("衝突音効")]
    public FMODUnity.EventReference dashedEffect;
    
}
