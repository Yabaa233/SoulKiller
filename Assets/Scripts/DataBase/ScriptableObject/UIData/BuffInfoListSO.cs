using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �������е�buff��Ϣ
/// xushi
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObject/UI/BuffInfoList")]
public class BuffInfoListSO : ScriptableObject
{
    //buff�б�
    public List<BuffItemData> buffItems;

}
/// <summary>
/// ����buff��ͼ�ꡢ��������Ϣ
/// </summary>
[System.Serializable]
public class BuffItemData
{
    //buff����
    public string buffName;
    //buff����
    public E_BuffKind buffKind;
    //buffͼ��
    public Sprite buffSprite;
    //buff����
    [TextArea]
    public string buffDescribe;
    //buff���±���
    [TextArea]
    public string buffStory;
    //buff�ȼ�
    public List<BuffLevelData> buffLevelDatas;

    //�ؿ����
    //ʤ������
    [TextArea]
    public string Levelcondition;
    //�ؿ���ʾ
    [TextArea]
    public string LevelTips;

    //buff����ͼ��
    public Sprite buffSpriteText;
}
/// <summary>
/// ����buff��ǰ�ȼ�����Ϣ
/// </summary>
[System.Serializable]
public class BuffLevelData
{
    //��ǰ�ȼ�
    public int curLevel;
    //�ȼ�����
    public string levelName;
    //�ȼ�ͼ��
    public Sprite levelSprite;
    //�ȼ�����
    [TextArea]
    public string levelDescribe;
}
