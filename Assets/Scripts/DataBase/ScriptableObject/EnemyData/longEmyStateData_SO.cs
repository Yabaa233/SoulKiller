using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// [CreateAssetMenu(menuName = "ScriptableObject/Battle/GunEmyStateData")]
///<summary>

////// 敵を遠距離から攻撃するための記述ファイル、すべての属性はここに配置されます。

///</summary>
public class longEmyStateData_SO : ScriptableObject
{
    [Header("攻撃距離")]
    public float attackDistance;
    [Header("攻撃間隔")]
    public float attackSpeed;
    [Header("基本移動速度")]
    public float moveSpeed;
    [Header("攻撃音効")]
    public FMODUnity.EventReference attackSound;
    [Header("フラッシュサウンドエフェクト")]

    public FMODUnity.EventReference shiftSound;

}
