using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetBuffPanel : BasePanel
{
    static readonly string path = "UI/Panel/GetBuffPanel";
    public GetBuffPanel() : base(new UIType(path)) { }

    //ԭ��ͼ
    Image buffImage1;
    Image buffImage2;
    Image buffImage3;
    //ʱ��
    Text time;

    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        buffImage1 = UITool.GetOrAddComponentInChildren<Image>("Img_Buff");
        buffImage2 = UITool.GetOrAddComponentInChildren<Image>("Img_Buff2");
        buffImage3 = UITool.GetOrAddComponentInChildren<Image>("Img_Buff3");
        time = UITool.GetOrAddComponentInChildren<Text>("Txt_Describe");

        List<BuffItemData> data = new List<BuffItemData>();

        foreach (var item in BuffDataManager.Instance.playerCurrentBuff)
        {
            foreach (var buffData in PanelManager.Instance.buffInfoListSO.buffItems)
            {
                if (item.buffKind == buffData.buffKind)
                {
                    data.Add(buffData);
                }
            }
        }

        buffImage1.sprite = data[0].buffSprite;
        buffImage2.sprite = data[1].buffSprite;
        buffImage3.sprite = data[2].buffSprite;

        //����ʱ��
        time.text = "ͨ通关时间" + ToTimeFormat(GameManager.Instance.GetClearTime());

        MonoHelper.Instance.StartCoroutine(SlowTime());
    }

    public string ToTimeFormat(float time)
    {
        //����ȡ��
        int seconds = (int)time;
        //һСʱΪ3600�� ������3600ȡ����ΪСʱ
        int hour = seconds / 3600;
        //һ����Ϊ60�� ������3600ȡ���ٶ�60ȡ����Ϊ����
        int minute = seconds % 3600 / 60;
        //��3600ȡ���ٶ�60ȡ�༴Ϊ����
        seconds = seconds % 3600 % 60;
        //����00:00:00ʱ���ʽ
        return string.Format("{0:D2}:{1:D2}:{2:D2}", hour, minute, seconds);
    }

    IEnumerator SlowTime()
    {
        Time.timeScale = 0.6f;
        float firstTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - firstTime < 4.0f)
        {
            yield return null;
        }

        Time.timeScale = 1f;
        Close();
    }

}
