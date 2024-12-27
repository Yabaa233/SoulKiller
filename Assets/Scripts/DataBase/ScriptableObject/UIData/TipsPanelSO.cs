using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 储存要展示的提示面板信息，用于ui显示
/// xushi
/// </summary>

[CreateAssetMenu(menuName = "ScriptableObject/UI/TipsPanel")]
public class TipsPanelSO : ScriptableObject
{
    public List<TipsPanelItem> tipsPanelItems;
}
[System.Serializable]
public class TipsPanelItem 
{
    //提示的名称
    //public string tipsName;
    //显示的图片
    public Sprite sprite;
    //显示的文字
    [TextArea]
    public string  text;
}
