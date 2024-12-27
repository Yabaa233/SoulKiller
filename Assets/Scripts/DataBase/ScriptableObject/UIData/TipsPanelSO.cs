using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ����Ҫչʾ����ʾ�����Ϣ������ui��ʾ
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
    //��ʾ������
    //public string tipsName;
    //��ʾ��ͼƬ
    public Sprite sprite;
    //��ʾ������
    [TextArea]
    public string  text;
}
