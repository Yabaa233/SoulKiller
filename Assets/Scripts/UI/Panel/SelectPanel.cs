using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ?????��???buff??????buff????
/// xushi
/// </summary>
public class SelectPanel : BasePanel
{
    static readonly string path = "UI/Panel/SelectPanel";

    public SelectPanel() : base(new UIType(path)) { }

    //?????????????????
    Text title;
    //buff???��?
    GameObject buffList;
    //????buff????
    Text buffNumber;
    //??buffinfo???ˮ?
    Button buffInfo;

    //??? ??????buff?��?
    GameObject leftArea;
    GameObject playerBuffList;
    //??? ????��?buff?��?
    GameObject rightArea;
    GameObject enemyBuffList;

    //????????????��????
    GameObject LeftFocusGO;
    //舍弃能力标题
    GameObject titleAbondon;
    //升级能力标题
    GameObject titleLevelUp;

    //舍弃能力背景
    GameObject backCardAbondon;
    //升级能力背景
    GameObject backCardLevelUP;

    //??????
    //??????????buff???????????????????????
    int abandonBuffLevel;
    //操作提示文字
    Text textTips;

    //打开abandon的协程
    Coroutine openAbandon;

    public override void OnShow(params object[] para)
    {


        base.OnShow(para);
        title = UITool.GetOrAddComponentInChildren<Text>("Txt_Title");
        buffList = UITool.FindChildGameObject("SelectArea(Scroll View").transform.Find("Viewport").Find("Content").gameObject;
        LeftFocusGO = UITool.FindChildGameObject("Img_LeftFocus");
        //buffNumber = UITool.GetOrAddComponentInChildren<Text>("Txt_curBuff");
        //buffInfo = UITool.GetOrAddComponentInChildren<Button>("Btn_BuffPanel");

        leftArea = UITool.FindChildGameObject("Img_LeftArea");
        rightArea = UITool.FindChildGameObject("Img_RightArea");
        playerBuffList = leftArea.transform.Find("Left(Scroll View").Find("Viewport").Find("Content").gameObject;
        enemyBuffList = rightArea.transform.Find("Right(Scroll View").Find("Viewport").Find("Content").gameObject;
        titleAbondon = UITool.FindChildGameObject("Img_Title");
        titleLevelUp = UITool.FindChildGameObject("Img_TitleLevelUp");
        backCardAbondon = UITool.FindChildGameObject("Img_BackCard");
        backCardLevelUP = UITool.FindChildGameObject("Img_BackCard3");

        textTips = UITool.GetOrAddComponentInChildren<Text>("Text_DD");
        //????
        //buffInfo.onClick.AddListener(() => { PanelManager.Instance.Open(new BuffInfoPanel()); });


        Time.timeScale = 0f;

        //test?????
        //title.text = (string)para[0];

        //buff???????????????buff????

        LeftFocusGO.SetActive(false);
        titleAbondon.SetActive(false);
        titleLevelUp.SetActive(false);
        backCardAbondon.SetActive(false);
        backCardLevelUP.SetActive(false);

        if (para.Length == 2)
            abandonBuffLevel = (int)para[1];

        if ((string)para[0] == "舍弃")
        {
            InitAbandonBuff();
            textTips.text = "长按舍弃";
        }
        else
        {
            InitLevelUp();
            textTips.text = "点击升级";
        }


    }

    public override void OnClose()
    {
        base.OnClose();
        Time.timeScale = 1f;
        //??????list?????list
        foreach (var item in PanelManager.Instance.GetAllPanel("Btn_SelectColumnBuff"))
        {
            item.Close();
        }
        foreach (var item in PanelManager.Instance.GetAllPanel("Btn_Abandon"))
        {
            item.Close();
        }
        foreach (var item in PanelManager.Instance.GetAllPanel("Btn_LevelUp"))
        {
            item.Close();
        }

    }

    //?????????????
    public void InitAbandonBuff()
    {
        //???????buff?????4??
        if (BuffDataManager.Instance.playerCurrentBuff.Count < 4)
        {
            Debug.Log("返回" + BuffDataManager.Instance.playerCurrentBuff.Count);
            Close();
            return;
        }

        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/panelOpen");

        titleAbondon.SetActive(true);
        titleLevelUp.SetActive(false);
        backCardAbondon.SetActive(true);
        backCardLevelUP.SetActive(false);

        //?????4??buff
        List<S_BuffKindAndLevel> buffShowList = new List<S_BuffKindAndLevel>();
        //????????????????
        List<S_BuffKindAndLevel> buffTempList = new List<S_BuffKindAndLevel>(BuffDataManager.Instance.playerCurrentBuff);

        //??????????buff?��?
        foreach (var item in buffTempList)
        {
            foreach (var buffItemData in PanelManager.Instance.buffInfoListSO.buffItems)
            {
                if (item.buffKind == buffItemData.buffKind)
                    PanelManager.Instance.Open(new Btn_SelectColumnBuff(), playerBuffList.transform, 0.5f, item.level, LeftFocusGO, buffItemData);
            }

        }
        foreach (var item in BuffDataManager.Instance.enemyBuffList)
        {
            foreach (var buffItemData in PanelManager.Instance.buffInfoListSO.buffItems)
            {
                if (item.GetBuffType() == buffItemData.buffKind)
                    PanelManager.Instance.Open(new Btn_SelectColumnBuff(), enemyBuffList.transform, 0.4f, item.GetLevel(), LeftFocusGO, buffItemData);
            }
        }

        //?��???????buff
        while (buffShowList.Count != 4 && buffTempList.Count != 0)
        {
            int index = Random.Range(0, buffTempList.Count);
            buffShowList.Add(buffTempList[index]);
            buffTempList.Remove(buffTempList[index]);
        }

        openAbandon = MonoHelper.Instance.StartCoroutine(LateOpenAbandon(buffShowList));
        coroutines.Add(openAbandon);



    }

    /// <summary>
    /// 延迟调用btn_abandon打开
    /// </summary>
    /// <returns></returns>
    IEnumerator LateOpenAbandon(List<S_BuffKindAndLevel> buffShowList)
    {
        foreach (var item in buffShowList)
        {
            foreach (var buffItem in PanelManager.Instance.buffInfoListSO.buffItems)
            {
                if (item.buffKind == buffItem.buffKind)
                    PanelManager.Instance.Open(new Btn_Abandon(), buffList.transform, buffItem);
            }

            float firstTime = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup - firstTime < 0.15f)
            {
                yield return null;
            }
        }
    }

    /// <summary>
    /// ???????????????????
    /// </summary>
    /// <param name="e_BuffKind"></param>
    public void HighLight(E_BuffKind e_BuffKind)
    {
        foreach (var item in PanelManager.Instance.GetAllPanel("Btn_SelectColumnBuff"))
        {
            item.Close();
        }

        //????????buff?��?
        foreach (var item in BuffDataManager.Instance.playerCurrentBuff)
        {

            if (item.buffKind == e_BuffKind)
            {
                foreach (var buffItemData in PanelManager.Instance.buffInfoListSO.buffItems)
                {
                    if (item.buffKind == buffItemData.buffKind)
                        PanelManager.Instance.Open(new Btn_SelectColumnBuff(), playerBuffList.transform, 1f, item.level, LeftFocusGO, buffItemData);
                }
            }
            else
            {
                foreach (var buffItemData in PanelManager.Instance.buffInfoListSO.buffItems)
                {
                    if (item.buffKind == buffItemData.buffKind)
                        PanelManager.Instance.Open(new Btn_SelectColumnBuff(), playerBuffList.transform, 0.5f, item.level, LeftFocusGO, buffItemData);
                }
            }
        }
        //????????buff?��?
        foreach (var item in BuffDataManager.Instance.enemyBuffList)
        {
            foreach (var buffItemData in PanelManager.Instance.buffInfoListSO.buffItems)
            {
                if (item.GetBuffType() == buffItemData.buffKind)
                    PanelManager.Instance.Open(new Btn_SelectColumnBuff(), enemyBuffList.transform, 0.4f, item.GetBuffType(), LeftFocusGO, buffItemData);
            }
        }
    }

    //?????buff????????
    public void InitLevelUp()
    {
        titleAbondon.SetActive(false);
        titleLevelUp.SetActive(true);
        backCardAbondon.SetActive(false);
        backCardLevelUP.SetActive(true);

        //?????3??buff
        List<S_BuffKindAndLevel> buffShowList = new List<S_BuffKindAndLevel>();
        //????��??????????????????
        List<S_BuffKindAndLevel> buffTempList = new List<S_BuffKindAndLevel>(BuffDataManager.Instance.playerCurrentBuff);

        //??????????buff?��?
        foreach (var item in buffTempList)
        {
            foreach (var buffItemData in PanelManager.Instance.buffInfoListSO.buffItems)
            {
                if (item.buffKind == buffItemData.buffKind)
                    PanelManager.Instance.Open(new Btn_SelectColumnBuff(), playerBuffList.transform, 0.5f, item.level, LeftFocusGO, buffItemData);
            }
        }
        foreach (var item in BuffDataManager.Instance.enemyBuffList)
        {
            foreach (var buffItemData in PanelManager.Instance.buffInfoListSO.buffItems)
            {
                if (item.GetBuffType() == buffItemData.buffKind)
                    PanelManager.Instance.Open(new Btn_SelectColumnBuff(), enemyBuffList.transform, 0.4f, item.GetLevel(), LeftFocusGO, buffItemData);
            }
        }

        //???��???????????????
        foreach (var item in BuffDataManager.Instance.playerCurrentBuff)
        {
            if (item.level == 4)
                buffTempList.Remove(item);
        }

        //?��???????��??????????
        if (buffTempList.Count == 0)
        {
            Debug.Log("????????��??????????");
            Close();
            return;
        }

        while (buffShowList.Count != 3 && buffTempList.Count != 0)
        {
            int index = Random.Range(0, buffTempList.Count);
            buffShowList.Add(buffTempList[index]);
            buffTempList.Remove(buffTempList[index]);
        }

        foreach (var item in buffShowList)
        {
            foreach (var buffItem in PanelManager.Instance.buffInfoListSO.buffItems)
            {
                //?????��buff????????buff?????????buff???
                if (item.buffKind == buffItem.buffKind)
                    PanelManager.Instance.Open(new Btn_LevelUp(), buffList.transform, item, buffItem, abandonBuffLevel);
            }

        }

        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/panelOpen");
    }
}
