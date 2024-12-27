using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 储存所有的buff信息
/// xushi
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObject/UI/BuffInfoList")]
public class BuffInfoListSO : ScriptableObject
{
    //buff列表
    public List<BuffItemData> buffItems;

}
/// <summary>
/// 储存buff的图标、描述等信息
/// </summary>
[System.Serializable]
public class BuffItemData
{
    //buff名称
    public string buffName;
    //buff种类
    public E_BuffKind buffKind;
    //buff图标
    public Sprite buffSprite;
    //buff描述
    [TextArea]
    public string buffDescribe;
    //buff故事背景
    [TextArea]
    public string buffStory;
    //buff等级
    public List<BuffLevelData> buffLevelDatas;

    //关卡相关
    //胜利条件
    [TextArea]
    public string Levelcondition;
    //关卡提示
    [TextArea]
    public string LevelTips;

    //buff文字图标
    public Sprite buffSpriteText;
}
/// <summary>
/// 储存buff当前等级的信息
/// </summary>
[System.Serializable]
public class BuffLevelData
{
    //当前等级
    public int curLevel;
    //等级名称
    public string levelName;
    //等级图标
    public Sprite levelSprite;
    //等级描述
    [TextArea]
    public string levelDescribe;
}
