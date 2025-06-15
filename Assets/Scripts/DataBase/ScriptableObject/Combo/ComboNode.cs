using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Combo")]
public class ComboNode : ScriptableObject
{
    [Header("基本状態")]
    [Tooltip("攻撃特効")] public GameObject attackEffect;    //攻撃特効
    [Tooltip("攻撃基本倍率")] public float baseDamage;    //技のダメージ倍率
    [Tooltip("攻撃基本範囲")] public Vector3 attackRange;    //基本的な範囲の技
    [Tooltip("攻撃範囲のオフセット")] public float attackRangeDeviation = 0;

    // [Tooltip("攻撃アニメーションが強制的に再生される時間")] public float stiffnessTime; //硬直時間
    [Tooltip("攻撃アニメーションの強制再生比率")][Range(0.0f, 1.0f)] public float forceAnimProgress; //強制的にアニメーションを再生する比率


    [Header("サウンドエフェクト")]
    [Tooltip("攻撃音効")] public FMODUnity.EventReference attackSound;  //攻撃音効
    [Tooltip("クリティカルヒット音效")] public FMODUnity.EventReference criticalSound;  //クリティカルヒット音效


    [Header("撃退効果")]
    [Tooltip("X Z方向のノックバック比率")] public Vector2 forceDir;    //撃退方向
    [Tooltip("反撃力の大きさ")] public float forcePower;    //反撃力の大きさ

    [Header("振動効果")]
    [Tooltip("振動タイプ")] public shake_type type;
    [Tooltip("振動持続時間")] public float shake_time;
    [Tooltip("振動振幅")] public float amp = 1;
    [Tooltip("振動周波数")] public float fre = 1;


    [Header("特別な運動状態")]
    [Tooltip("補償速度")] public float compensateSpeed;   //補償速度
    [Tooltip("強制的な移動速度")] public Vector2 forceSpeed;  //強制的な移動速度
    [Tooltip("自動敵検出ボックスのサイズの半分")] public Vector3 halfPlungeBoxSize;    //索敌ボックスのサイズの半分
    [Tooltip("自動で敵を探し、敵を追撃する速度")] public float plungePower; //自動で敵を探し、敵を追撃する速度

}
