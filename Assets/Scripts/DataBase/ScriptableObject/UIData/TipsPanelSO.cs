using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// UI表示用に表示するプロンプトパネル情報を保存します。
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
    //ヒントの名前
    //public string tipsName;
    //表示される画像
    public Sprite sprite;
    //表示されるテキスト
    [TextArea]
    public string  text;
}
