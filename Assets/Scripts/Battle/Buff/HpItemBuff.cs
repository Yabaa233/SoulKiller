using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



//Hp道具给予的Buff
[Serializable]
public class HpItemBuff : I_BuffBase
{
    [Header("外部传参获得数据部分")]
    //存当前角色的引用
    [Tooltip("当前角色引用")]GameObject buffKeeper;
    //存当前角色的BuffManager
    [Tooltip("当前角色Buff管理器")]CharacterBuffManager characterBuffManager;
    //当前BUFF种类
    [Tooltip("当前Buff类型")]E_BuffKind buffType;
    //当前角色的类型
    [Tooltip("当前角色类型")]E_ChararcterType chararcterType;
    //当前Buff叠加的层数->level 这里就不改名称了
    [Tooltip("当前Buff等级")]public int currentLevel;
    [Tooltip("掉血Buff的CD")]public CDClass HpItemBuffCD;
    [Tooltip("掉血Buff的持续时间List")]public List<CDClass> HpItemBuffKeepCDList = new List<CDClass>();

    public HpItemBuff(E_ChararcterType _chararcterType,int level = 0)
    {
        //赋值
        this.currentLevel = level;
        buffType = E_BuffKind.HpItemBuff;
        chararcterType = _chararcterType;

        //添加一些统一的CD
        HpItemBuffCD = new CDClass();
        HpItemBuffCD.maxCDTime = 1;
        HpItemBuffCD.flag = true;
        GameManager.Instance.CDList.Add(HpItemBuffCD);

    }

    public void OnAdd(GameObject _buffKeeper)
    {
        //添加持续时间
        CDClass temp = new CDClass();
        temp.maxCDTime = 20;
        temp.flag = false;
        HpItemBuffKeepCDList.Add(temp);
        
        //增加一层层数
        currentLevel += 1;

        this.buffKeeper = _buffKeeper;
        switch(chararcterType)
        {
            case E_ChararcterType.player:PlayerHpItemBuff();break;
            case E_ChararcterType.enemy:break;
            case E_ChararcterType.boss:break; 
        }
    }

    public void OnUpdate(float deltaTime)
    {
        CDUpdate();//更新当前CD，处理列表
        if(currentLevel <=0)//没有层数了就移除这个Buff
        {
            SelfRemove();
            return;
        }
        if(HpItemBuffCD.flag)
        {
            switch(chararcterType)
            {
                case E_ChararcterType.player:UpdatePlayerHpItemBuff();break;
                case E_ChararcterType.enemy:break;
                case E_ChararcterType.boss:break; 
            }
            HpItemBuffCD.flag = false;
        }
    }

    //更新CD
    public void CDUpdate()
    {
        List<CDClass> HpItemFuzhu = new List<CDClass>(HpItemBuffKeepCDList); //遍历辅助数组，移除原数组的元素
        foreach (CDClass temp in HpItemFuzhu.ToArray())
        {
            if (!temp.flag && temp.curTime < temp.maxCDTime)
            {
                temp.curTime += Time.deltaTime;
                if (temp.curTime > temp.maxCDTime)
                {
                    HpItemBuffKeepCDList.Remove(temp);
                    currentLevel -=1 ;//减少一层Buff
                }
            }
        }
    }

    public void OnRemove()
    {
        
    }

    public E_BuffKind GetBuffType()
    {
        return buffType;
    }

    public GameObject GetBuffKeeper()
    {
        return buffKeeper;
    }

    public E_ChararcterType GetChararcterType()
    {
        return chararcterType;
    }

    public int GetLevel()
    {
        return currentLevel;
    }

////////////
    public void PlayerHpItemBuff()//OnAdd
    {
        
        PlayerControl playerControl = buffKeeper.GetComponent<PlayerControl>();
        CharacterData characterData = playerControl.characterData;

        characterData.currentHealth += BuffDataManager.Instance.playerRaiseHealthy;
        if(characterData.currentHealth > characterData.maxHealth)
        {
            characterData.currentHealth = characterData.maxHealth;
        }
    }

    public void UpdatePlayerHpItemBuff()//Update
    {
        PlayerControl playerControl = buffKeeper.GetComponent<PlayerControl>();
        CharacterData characterData = playerControl.characterData;

        float damage = BuffDataManager.Instance.playerHpReduce * currentLevel;
        characterData.currentHealth -= damage;

        PanelManager.Instance.GenerateDamageNum(damage,playerControl.gameObject.transform,false,true);//产生伤害数字 

        GameManager.Instance.PlayerHealthCheck();//检查玩家血量
    }

    //自动移除方法
    public void SelfRemove()
    {
        switch(chararcterType)
        {
            case E_ChararcterType.player:PlayerSelfRemove();break;
            case E_ChararcterType.enemy:break;
            case E_ChararcterType.boss:break; 
        }
    }

    public void PlayerSelfRemove()
    {
        Debug.Log("执行了移除");
        PlayerControl playerControl = buffKeeper.GetComponent<PlayerControl>();
        playerControl.characterBuffManager.RemoveBuff(this);
    }
    
}
