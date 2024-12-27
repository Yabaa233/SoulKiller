using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
public enum E_ChararcterType
{
    player,
    enemy,
    boss
}

[System.Serializable]
public class CharacterBuffManager
{
    [Header("当前角色类型")]
    public E_ChararcterType type;
    [SerializeField]
    [Tooltip("当前角色持有的BUff列表")] public List<I_BuffBase> characterKeepBuffList = new List<I_BuffBase>();
    [SerializeField]
    [Tooltip("字符和对应索引的数组")] public Dictionary<E_BuffKind, I_BuffBase> indexDictionary = new Dictionary<E_BuffKind, I_BuffBase>();
    //其实一开始就改用下方这种方式去做所有的Buff存储！
    // [SerializeField]
    // [Tooltip("持续性的Buff存储位置")]public Dictionary<E_BuffKind,List<I_BuffBase>> timeBuffDictionary = new Dictionary<E_BuffKind, List<I_BuffBase>>();

    //一些有实体的Buff应该在的位置
    [Header("护盾实体")] public ShieldRipples shieldRipples;
    /// <summary>
    /// 从BuffManager中获取所有Buff类型，并初始化Buff列表
    /// </summary>
    public void Init(E_ChararcterType _type)
    {
        this.type = _type;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="buff">要添加的Buff</param>
    /// <param name="gameObject">对象</param>
    public void AddBuff(I_BuffBase buff, GameObject gameObject,bool isKeppBuff = false)
    {
        if(isKeppBuff)//如果是持续性的Buff
        {
            if (!indexDictionary.ContainsKey(buff.GetBuffType()))
            {
                buff.OnAdd(gameObject);//一次性添加方法
                characterKeepBuffList.Add(buff);
                indexDictionary.Add(buff.GetBuffType(), buff);
            }
            else
            {
                buff.OnAdd(gameObject);//一次性添加方法
            }
        }
        else//如果不是持续性的
        {
            if (indexDictionary.ContainsKey(buff.GetBuffType()))
            {
                Debug.LogWarning("添加失败,你正在尝试添加已有的Buff");
                return;
            }
            buff.OnAdd(gameObject);//一次性添加方法
            characterKeepBuffList.Add(buff);
            indexDictionary.Add(buff.GetBuffType(), buff);
        }
    }

    /// <summary>
    /// 移除Buff的办法,知道具体是哪个Buff的时候
    /// </summary>
    /// <param name="buff">Buff实例</param>
    public void RemoveBuff(I_BuffBase buff)
    {
        // Debug.Log("调用REMOVE1");
        buff.OnRemove();
        characterKeepBuffList.Remove(buff);
        indexDictionary.Remove(buff.GetBuffType());
    }
    /// <summary>
    /// 移除Buff的办法，只知道Buff类型的时候
    /// </summary>
    /// <param name="buffKind">buff类型</param>
    public void RemoveBuff(E_BuffKind buffKind)
    {
        // Debug.Log("调用rEMOVE2");
        if (!indexDictionary.ContainsKey(buffKind))
        {
            Debug.LogWarning("没有对应的Buff,无法删除");
            return;
        }
        I_BuffBase buff = indexDictionary[buffKind];
        buff.OnRemove();
        characterKeepBuffList.Remove(buff);
        indexDictionary.Remove(buffKind);
    }

    /// <summary>
    /// 查找当前角色是否有某种Buff
    /// </summary>
    /// <param name="buffKind">Buff类型的枚举</param>
    /// <returns></returns>
    public bool FindBuff(E_BuffKind buffKind)
    {
        if(!indexDictionary.ContainsKey(buffKind))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// 通过反射将对应Buff提升至对应等级
    /// </summary>
    /// <param name="buffKind">Buff类型</param>
    /// <param name="level">等级</param>
    /// <param name="gameObject">持有Buff的对象</param>
    public void BuffLevelTo(E_BuffKind buffKind, int level, GameObject gameObject)
    {
        if (!indexDictionary.ContainsKey(buffKind))
        {
            Debug.LogWarning("没有对应的Buff,无法进行升级");
            return;
        }
        //得到一些变量
        I_BuffBase buff = indexDictionary[buffKind];
        GameObject buffKeeper = buff.GetBuffKeeper();
        E_ChararcterType chararcterType = buff.GetChararcterType();
        //执行一次移除
        buff.OnRemove();
        characterKeepBuffList.Remove(buff);
        indexDictionary.Remove(buffKind);

        //通过反射重新构建一次Buff
        Assembly assembly = Assembly.GetExecutingAssembly();
        Type t = buff.GetType();
        object[] o = {chararcterType, level};
        I_BuffBase obj = assembly.CreateInstance(t.ToString(), true, BindingFlags.Default, null, o, null, null) as I_BuffBase;
        AddBuff(obj, gameObject);
        BuffDataManager.Instance.playerBuffList = new List<I_BuffBase>(characterKeepBuffList);
        gameObject.GetComponent<PlayerControl>().PlayerBuffRebuild(BuffDataManager.Instance.playerBuffList);//需要同步一下数据
        // Debug.Log("Buff已经升级");
    }

    /// <summary>
    /// 重建Buff列表并且赋值
    /// </summary>
    /// <param name="newBuffList">新Buff列表</param>
    /// <param name="gameObject">要加给哪个物体</param>
    public void BuffReBuild(List<I_BuffBase> newBuffList, GameObject gameObject)
    {
        // Debug.Log("调用Rebuild");
        foreach (var buff in characterKeepBuffList)
        {
            buff.OnRemove();
        }
        characterKeepBuffList.Clear();
        indexDictionary.Clear();

        List<S_BuffKindAndLevel> currentBuffDic = new List<S_BuffKindAndLevel>();
        foreach (var buff in newBuffList)
        {
            buff.OnAdd(gameObject);
            characterKeepBuffList.Add(buff);
            indexDictionary.Add(buff.GetBuffType(), buff);
            //塞进结构体里面传递
            S_BuffKindAndLevel s = new S_BuffKindAndLevel();
            s.buffKind = buff.GetBuffType();
            s.level = buff.GetLevel();
            currentBuffDic.Add(s);
        }

        //同步单例类里面的数据，这样就不用在其它地方单独同步
        if (newBuffList.Count == 0)
        {
            Debug.LogWarning("新的Buff个数为0,请确认");
            // BuffDataManager.Instance.playerCurrentBuff.Clear();
            // BuffDataManager.Instance.enemyCurrentBuff.Clear();
        }
        else
        {
            E_ChararcterType chararcterType = newBuffList[0].GetChararcterType();
            if (chararcterType == E_ChararcterType.player)
            {
                BuffDataManager.Instance.playerCurrentBuff.Clear();
                BuffDataManager.Instance.playerCurrentBuff = currentBuffDic;
                //同步单例类的数据存储
                BuffDataManager.Instance.playerBuffList = new List<I_BuffBase>(characterKeepBuffList);
            }
            else if (chararcterType == E_ChararcterType.enemy)
            {
                BuffDataManager.Instance.enemyCurrentBuff.Clear();
                BuffDataManager.Instance.enemyCurrentBuff = currentBuffDic;
                //同步单例类的数据存储
                BuffDataManager.Instance.enemyBuffList = new List<I_BuffBase>(characterKeepBuffList);
            }
        }
        
    }
    /// <summary>
    /// 移除所有的Buff
    /// </summary>
    public void RemoveAllBuff()
    {
        foreach (var buff in characterKeepBuffList)
        {
            buff.OnRemove();
        }
        characterKeepBuffList.Clear();
        indexDictionary.Clear();
    }
    /// <summary>
    /// 同步玩家/敌人/Boss和数据类的数据交互
    /// </summary>
    public void RefreshData()
    {
        // Debug.Log("调用Refresh");
        List<S_BuffKindAndLevel> currentBuffDic = new List<S_BuffKindAndLevel>();
        foreach (var buff in characterKeepBuffList)
        {
            S_BuffKindAndLevel s = new S_BuffKindAndLevel();
            s.buffKind = buff.GetBuffType();
            s.level = buff.GetLevel();
            currentBuffDic.Add(s);
        }
        if (currentBuffDic.Count == 0)
        {
            Debug.LogWarning("新的Buff个数为0,请确认");
            // BuffDataManager.Instance.playerCurrentBuff.Clear();
            // BuffDataManager.Instance.enemyCurrentBuff.Clear();
        }
        else
        {
            E_ChararcterType chararcterType = characterKeepBuffList[0].GetChararcterType();
            if (chararcterType == E_ChararcterType.player)
            {
                BuffDataManager.Instance.playerCurrentBuff.Clear();
                BuffDataManager.Instance.playerCurrentBuff = currentBuffDic;
                //同步单例类的数据存储
                BuffDataManager.Instance.playerBuffList = new List<I_BuffBase>(characterKeepBuffList);
            }
            else if (chararcterType == E_ChararcterType.enemy)
            {
                BuffDataManager.Instance.enemyCurrentBuff.Clear();
                BuffDataManager.Instance.enemyCurrentBuff = currentBuffDic;
                //同步单例类的数据存储
                BuffDataManager.Instance.enemyBuffList = new List<I_BuffBase>(characterKeepBuffList);
            }
        }

    }
    /// <summary>
    /// 分发对应的Update事件
    /// </summary>
    public void OnUpdate(float deltaTime)
    {
        List<I_BuffBase> characterKeepBuffListFuzhu = new List<I_BuffBase>(characterKeepBuffList);
        foreach (var buff in characterKeepBuffListFuzhu)
        {
            buff.OnUpdate(deltaTime);//在这里对应调用每一个Buff的方法
        }
    }

    /// <summary>
    /// 清空所有减益状态，现在只有掉血Buff所以可以单独修改
    /// </summary>
    public void ClearDebuff()
    {
        //掉血Buff移除
        RemoveBuff(E_BuffKind.HpItemBuff);
    }


    /// /////////一些特殊Buff的实现部分


    /// <summary>
    /// 吸血返还的生命值
    /// </summary>
    /// <param name="damage">当次攻击的伤害值</param>
    public void ReturnHP(float damage)
    {
        if (!indexDictionary.ContainsKey(E_BuffKind.HpUp))
        {
            // Debug.LogWarning("没有找到HpBuff,无法吸血");
            return;
        }
        //如果强制转换不安全的话，可以通过GetLevel判断当前技能等级是否>=吸血开启等级
        HpUp buff = indexDictionary[E_BuffKind.HpUp] as HpUp;
        buff.ReturnHp(damage);
    }

    /// <summary>
    /// 得到玩家冲刺应当有的次数
    /// </summary>
    public int GetDogeTimes()
    {
        if (!indexDictionary.ContainsKey(E_BuffKind.SpeedBuff))
        {
            return 1;//如果没有该Buff的话就只能冲刺一次
        }
        SpeedBuff buff = indexDictionary[E_BuffKind.SpeedBuff] as SpeedBuff;
        return buff.GetDogeTimes();
    }

    /// <summary>
    /// 判定是否有护盾的接口,true代表有护盾，false代表没有护盾
    /// </summary>
    /// <param name="characterData">攻击方的数据</param>
    /// <param name="damage">该次攻击伤害</param>
    /// <returns></returns>
    public bool CalcuSheild(CharacterData attackData,float damage)
    {
        bool result = false;
        if(!indexDictionary.ContainsKey(E_BuffKind.ShieldBuff))//如果没有护盾，则返回
        {
            // Debug.Log("没有护盾了");
            return result;
        }
        if(shieldRipples == null)
        {
            // Debug.LogWarning("Buff已经被删除了");
            return result;
        }
        if(shieldRipples.currentHealth<=0)
        {
            return result;
        }
        ShieldBuff buff = indexDictionary[E_BuffKind.ShieldBuff] as ShieldBuff;
        FMODUnity.RuntimeManager.PlayOneShot("event:/Monster/NorMal/shierdHit");
        //反伤
        if(buff.GetLevel()>=4 && type == E_ChararcterType.player)
        {
            buff.DamageReflect(attackData,damage);
        }

        //如果有盾
        if(shieldRipples.currentHealth >0)
        {
            result = true;
        }
        //对盾造成伤害
        shieldRipples.currentHealth -= damage;
        //破碎
        if(shieldRipples.currentHealth <= 0)
        {
            shieldRipples.currentHealth = 0;
            if(shieldRipples.isTrue)
            {
                shieldRipples.isTrue = false;
                shieldRipples.DestroyShield();
            }
            if(type == E_ChararcterType.player && buff.GetLevel()>=2 && shieldRipples.isTrue)//玩家的盾破碎后造成伤害
            {
                shieldRipples.ShieldDamage();
                FMODUnity.RuntimeManager.PlayOneShot("event:/Monster/NorMal/shierdBoom");
                // shieldRipples.isTrue = false;
                // shieldRipples.SetShieldVisble(false);
            }
            // RemoveBuff(buff);
            //销毁护盾条
            // shieldRipples.DestroyShield();
            //设置可见性
            // shieldRipples.isTrue = false;
            // shieldRipples.SetShieldVisble(false);
            // shieldRipples = null;
            // FMODUnity.RuntimeManager.PlayOneShot("event:/Monster/NorMal/shierdBoom");         
        }
        return result;
    }

    /// <summary>
    /// 判断当前护盾是否还在生效,true代表生效，false代表没生效
    /// </summary>
    /// <returns></returns>
    public bool HasShield()
    {
        if(!indexDictionary.ContainsKey(E_BuffKind.ShieldBuff))//如果没有护盾，则返回
        {
            // Debug.Log("没有护盾了");
            return false;
        }
        if(shieldRipples == null)
        {
            // Debug.LogWarning("Buff已经被删除了");
            return false;
        }
        return shieldRipples.isTrue;
    }

    /// <summary>
    /// 返回玩家可以连击的次数
    /// </summary>
    /// <returns></returns>
    public int PlayerSwordTimes()
    {
        if(type != E_ChararcterType.player)
        {
            Debug.LogWarning("注意,你正在为非玩家角色获取连击次数");
            return 0;
        }
        
        if (!indexDictionary.ContainsKey(E_BuffKind.SwordBuff))
        {
            return 2;//当前的策划案是不能丢弃剑，所以最低是两次连击次数
        }

        SwordBuff buff = indexDictionary[E_BuffKind.SwordBuff] as SwordBuff;
        return buff.GetPlayerTimes();
    }
    /// <summary>
    /// 返回角色剑Buff等级
    /// </summary>
    /// <returns></returns>
    public int PlayerSwordLevel()
    {
        if(type != E_ChararcterType.player)
        {
            Debug.LogWarning("注意,你正在为非玩家角色获取剑等级");
            return 0;
        }
        if (!indexDictionary.ContainsKey(E_BuffKind.SwordBuff))
        {
            return 0;
        }
        SwordBuff buff = indexDictionary[E_BuffKind.SwordBuff] as SwordBuff;
        return buff.currentLevel;
    }

    /// <summary>
    /// 返回玩家当前枪的Buff等级,0代表没有这个Buff
    /// </summary>
    /// <returns></returns>
    public int PlayerGunBuffLevel()
    {
        if(type != E_ChararcterType.player)
        {
            Debug.LogWarning("注意,你正在为非玩家角色获取枪械Buff等级");
            return 0;
        }
        if(!indexDictionary.ContainsKey(E_BuffKind.GunBuff))
        {
            return 0;
        }

        GunBuff buff = indexDictionary[E_BuffKind.GunBuff] as GunBuff;
        return buff.GetLevel();
    }

    /// <summary>
    /// 返回玩家当前法杖的Buff等级，0代表没有这个Buff
    /// </summary>
    /// <returns></returns>
    public int PlayerStaffBuffLevel()
    {
        if(type != E_ChararcterType.player)
        {
            Debug.LogWarning("注意,你正在为非玩家角色获取法杖Buff等级");
            return 0;
        }
        if(!indexDictionary.ContainsKey(E_BuffKind.StaffBuff))
        {
            return 0;
        }

        StaffBuff buff = indexDictionary[E_BuffKind.StaffBuff] as StaffBuff;
        return buff.GetLevel();
    }

    /// <summary>
    /// 回复护盾值
    /// </summary>
    /// <param name="raise">护盾的回复量</param>
    public void RaiseShieldHP(float raise)
    {
        if(!indexDictionary.ContainsKey(E_BuffKind.ShieldBuff))
        {
            Debug.LogWarning("当前角色没有护盾");
            return;
        }
        shieldRipples.currentHealth += raise;
        if(shieldRipples.currentHealth > shieldRipples.maxHealth)
        {
            shieldRipples.currentHealth = shieldRipples.maxHealth;
        }

        if(shieldRipples.currentHealth > 0)
        {
            shieldRipples.isTrue = true;
            shieldRipples.SetShieldVisble(true);
        }
    }

    /// <summary>
    /// 添加HpItemBuff,做一层封装，通过这个函数可以减少创建buff的次数
    /// </summary>
    /// <param name="chararcterType">角色类型</param>
    /// <param name="gameObject">Buff持有者</param>
    public void AddHpItemBuff(E_ChararcterType chararcterType,GameObject gameObject)
    {
        HpItemBuff hpItemBuff;
        if(!indexDictionary.ContainsKey(E_BuffKind.HpItemBuff))
        {
            hpItemBuff = new HpItemBuff(chararcterType);
        }
        else
        {
            hpItemBuff = indexDictionary[E_BuffKind.HpItemBuff] as HpItemBuff;
        }
        AddBuff(hpItemBuff,gameObject,true);
    }
    
    //这个是为了以后如果有其他的道具Buff准备的接口，方法为传入类型，如果字典里没有实例，则创建，否则直接再调用一次Add
    public void AddItemBuff(E_BuffKind buffKind)
    {

    }

}
