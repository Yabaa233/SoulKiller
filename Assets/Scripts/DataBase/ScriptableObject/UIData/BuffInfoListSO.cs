using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// すべてのバフ情報を保存する
/// xushi
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObject/UI/BuffInfoList")]
public class BuffInfoListSO : ScriptableObject
{
    //バフリスト
    public List<BuffItemData> buffItems;

}
/// <summary>
/// バフのアイコン、説明などの情報を保存します。
/// </summary>
[System.Serializable]
public class BuffItemData
{
    //バフ名
    public string buffName;
    //バフの種類
    public E_BuffKind buffKind;
    //バフアイコン
    public Sprite buffSprite;
    //バフの説明
    [TextArea]
    public string buffDescribe;
    //バフのストーリーバックグラウンド
    [TextArea]
    public string buffStory;
    //バフレベル
    public List<BuffLevelData> buffLevelDatas;

    //ステージ関連
    //勝利条件
    [TextArea]
    public string Levelcondition;
    //ステージのヒント
    [TextArea]
    public string LevelTips;

    //バフテキストアイコン
    public Sprite buffSpriteText;
}
///<summary>

////// 現在のバフレベルの情報を保存する

///</summary>
[System.Serializable]
public class BuffLevelData
{
    //現在のレベル
    public int curLevel;
    //ランク名
    public string levelName;
    //ランクアイコン
    public Sprite levelSprite;
    //レベルの説明
    [TextArea]
    public string levelDescribe;
}
