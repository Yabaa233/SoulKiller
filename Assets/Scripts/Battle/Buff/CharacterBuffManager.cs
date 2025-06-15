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
    [Header("Current character type")]
    public E_ChararcterType type;
    [SerializeField]
    [Tooltip("The Buff list currently held by the character.")] public List<I_BuffBase> characterKeepBuffList = new List<I_BuffBase>();
    [SerializeField]
    [Tooltip("An array of text and corresponding indices")] public Dictionary<E_BuffKind, I_BuffBase> indexDictionary = new Dictionary<E_BuffKind, I_BuffBase>();
    //実際には、最初からこの方法を使ってすべてのバフを保存するんだ！
    // [SerializeField]
    // [Tooltip("Storage location for persistent Buff")]public Dictionary<E_BuffKind,List<I_BuffBase>> timeBuffDictionary = new Dictionary<E_BuffKind, List<I_BuffBase>>();

    //実体のあるバフが存在する場所
    [Header("Shield Entity")] public ShieldRipples shieldRipples;
    /// <summary>
    /// バフマネージャーからすべてのバフタイプを取得し、バフリストを初期化します。
    /// </summary>
    public void Init(E_ChararcterType _type)
    {
        this.type = _type;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="buff">追加するバフ</param>
    /// <param name="gameObject">オブジェクト</param>
    public void AddBuff(I_BuffBase buff, GameObject gameObject,bool isKeppBuff = false)
    {
        if(isKeppBuff)//もし継続的なバフなら
        {
            if (!indexDictionary.ContainsKey(buff.GetBuffType()))
            {
                buff.OnAdd(gameObject);//一時的な追加方法
                characterKeepBuffList.Add(buff);
                indexDictionary.Add(buff.GetBuffType(), buff);
            }
            else
            {
                buff.OnAdd(gameObject);//一時的な追加方法
            }
        }
        else//もし継続的でなければ
        {
            if (indexDictionary.ContainsKey(buff.GetBuffType()))
            {
                Debug.LogWarning("Failed to append, trying to add an already existing Buff.");
                return;
            }
            buff.OnAdd(gameObject);//一時的な追加方法
            characterKeepBuffList.Add(buff);
            indexDictionary.Add(buff.GetBuffType(), buff);
        }
    }

    /// <summary>
    /// バフを削除する方法、具体的にはどのバフか知っているとき
    /// </summary>
    /// <param name="buff">バフインスタンス</param>
    public void RemoveBuff(I_BuffBase buff)
    {
        // Debug.Log("Call REMOVE1.");
        buff.OnRemove();
        characterKeepBuffList.Remove(buff);
        indexDictionary.Remove(buff.GetBuffType());
    }
    /// <summary>
    /// バフを削除する方法、バフのタイプだけを知っている場合
    /// </summary>
    /// <param name="buffKind">バフタイプ</param>
    public void RemoveBuff(E_BuffKind buffKind)
    {
        // Debug.Log("Call rEMOVE2.");
        if (!indexDictionary.ContainsKey(buffKind))
        {
            Debug.LogWarning("Cannot delete because there is no corresponding buff.");
            return;
        }
        I_BuffBase buff = indexDictionary[buffKind];
        buff.OnRemove();
        characterKeepBuffList.Remove(buff);
        indexDictionary.Remove(buffKind);
    }

    ///<summary>


    ////// 現在のキャラクターが特定のバフを持っているかどうかを確認します。


    ///</summary>
    /// <param name="buffKind">バフタイプの列挙</param>
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
    /// リフレクションを使用して、対応するBuffを対応するレベルにアップグレードします。
    /// </summary>
    /// <param name="buffKind">バフの種類</param>
    /// <param name="level">レベル</param>
    /// <param name="gameObject">バフを持つオブジェクト</param>
    public void BuffLevelTo(E_BuffKind buffKind, int level, GameObject gameObject)
    {
        if (!indexDictionary.ContainsKey(buffKind))
        {
            Debug.LogWarning("There is no corresponding Buff, unable to upgrade.");
            return;
        }
        //いくつかの変数を取得する
        I_BuffBase buff = indexDictionary[buffKind];
        GameObject buffKeeper = buff.GetBuffKeeper();
        E_ChararcterType chararcterType = buff.GetChararcterType();
        //一度削除を実行する
        buff.OnRemove();
        characterKeepBuffList.Remove(buff);
        indexDictionary.Remove(buffKind);

        //リフレクションを通じてBuffを再構築します
        Assembly assembly = Assembly.GetExecutingAssembly();
        Type t = buff.GetType();
        object[] o = {chararcterType, level};
        I_BuffBase obj = assembly.CreateInstance(t.ToString(), true, BindingFlags.Default, null, o, null, null) as I_BuffBase;
        AddBuff(obj, gameObject);
        BuffDataManager.Instance.playerBuffList = new List<I_BuffBase>(characterKeepBuffList);
        gameObject.GetComponent<PlayerControl>().PlayerBuffRebuild(BuffDataManager.Instance.playerBuffList);//データを同期する必要があります。
        // Debug.Log("Buffがアップグレードされました");
    }

    /// <summary>
    /// Buffリストを再構築し、値を割り当てます
    /// </summary>
    /// <param name="newBuffList">新しいBuffリスト</param>
    /// <param name="gameObject">どのオブジェクトに追加するか</param>
    public void BuffReBuild(List<I_BuffBase> newBuffList, GameObject gameObject)
    {
        // Debug.Log("Call Rebuild");
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
            //構造体に詰めて渡す
            S_BuffKindAndLevel s = new S_BuffKindAndLevel();
            s.buffKind = buff.GetBuffType();
            s.level = buff.GetLevel();
            currentBuffDic.Add(s);
        }

        //同期シングルトンクラス内のデータを同期することで、他の場所で個別に同期する必要がなくなります。
        if (newBuffList.Count == 0)
        {
            Debug.LogWarning("The number of new Buffs is 0, please confirm.");
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
                //同期シングルトンクラスのデータストレージ
                BuffDataManager.Instance.playerBuffList = new List<I_BuffBase>(characterKeepBuffList);
            }
            else if (chararcterType == E_ChararcterType.enemy)
            {
                BuffDataManager.Instance.enemyCurrentBuff.Clear();
                BuffDataManager.Instance.enemyCurrentBuff = currentBuffDic;
                //同期シングルトンクラスのデータストレージ
                BuffDataManager.Instance.enemyBuffList = new List<I_BuffBase>(characterKeepBuffList);
            }
        }
        
    }
    /// <summary>
    /// すべてのバフを削除します
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
    /// プレイヤー/敵/Bossおよびデータクラスのデータ交換を同期します
    /// </summary>
    public void RefreshData()
    {
        // Debug.Log("Call Refresh");
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
            Debug.LogWarning("The number of new Buffs is 0, please confirm.");
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
                //同期シングルトンクラスのデータストレージ
                BuffDataManager.Instance.playerBuffList = new List<I_BuffBase>(characterKeepBuffList);
            }
            else if (chararcterType == E_ChararcterType.enemy)
            {
                BuffDataManager.Instance.enemyCurrentBuff.Clear();
                BuffDataManager.Instance.enemyCurrentBuff = currentBuffDic;
                //同期シングルトンクラスのデータストレージ
                BuffDataManager.Instance.enemyBuffList = new List<I_BuffBase>(characterKeepBuffList);
            }
        }

    }
    ///<summary>

    ////// 対応するUpdateイベントを配布します

    ///</summary>
    public void OnUpdate(float deltaTime)
    {
        List<I_BuffBase> characterKeepBuffListFuzhu = new List<I_BuffBase>(characterKeepBuffList);
        foreach (var buff in characterKeepBuffListFuzhu)
        {
            buff.OnUpdate(deltaTime);//ここでは、各Buffのメソッドを対応するように呼び出します。
        }
    }

    /// <summary>
    /// すべてのデバフをクリアし、現在はHP減少のバフのみとなっているため、個別に修正することが可能です。
    /// </summary>
    public void ClearDebuff()
    {
        //血を失うバフが削除されました
        RemoveBuff(E_BuffKind.HpItemBuff);
    }


    /// 一部の特別なバフの実装


    ///<summary>



    ////// 吸血によって回復するライフポイント



    ///</summary>
    /// <param name="damage">この攻撃のダメージ値</param>
    public void ReturnHP(float damage)
    {
        if (!indexDictionary.ContainsKey(E_BuffKind.HpUp))
        {
            // Debug.LogWarning("HpBuff cannot be found, unable to drain health.");
            return;
        }
        //強制的な変換が安全でない場合、GetLevelを通じて現在のスキルレベルが吸血開始レベル以上であるかどうかを判断することができます。
        HpUp buff = indexDictionary[E_BuffKind.HpUp] as HpUp;
        buff.ReturnHp(damage);
    }

    /// <summary>
    /// プレイヤーがスプリントすべき回数を得る
    /// </summary>
    public int GetDogeTimes()
    {
        if (!indexDictionary.ContainsKey(E_BuffKind.SpeedBuff))
        {
            return 1;//そのバフがなければ、ダッシュは一度だけしかできません。
        }
        SpeedBuff buff = indexDictionary[E_BuffKind.SpeedBuff] as SpeedBuff;
        return buff.GetDogeTimes();
    }

    /// <summary>
    /// シールドの有無を判断するインターフェースで、trueはシールドが存在することを示し、falseはシールドが存在しないことを示します。
    /// </summary>
    /// <param name="characterData">攻撃者のデータ</param>
    /// <param name="damage">この攻撃のダメージ</param>
    /// <returns></returns>
    public bool CalcuSheild(CharacterData attackData,float damage)
    {
        bool result = false;
        if(!indexDictionary.ContainsKey(E_BuffKind.ShieldBuff))//シールドがなければ、戻ってください。
        {
            // Debug.Log("シールドがなくなりました");
            return result;
        }
        if(shieldRipples == null)
        {
            // Debug.LogWarning("The buff has already been removed.");
            return result;
        }
        if(shieldRipples.currentHealth<=0)
        {
            return result;
        }
        ShieldBuff buff = indexDictionary[E_BuffKind.ShieldBuff] as ShieldBuff;
        FMODUnity.RuntimeManager.PlayOneShot("event:/Monster/NorMal/shierdHit");
        //反撃ダメージ
        if(buff.GetLevel()>=4 && type == E_ChararcterType.player)
        {
            buff.DamageReflect(attackData,damage);
        }

        //もし盾があれば
        if(shieldRipples.currentHealth >0)
        {
            result = true;
        }
        //シールドにダメージを与える
        shieldRipples.currentHealth -= damage;
        //壊れた
        if(shieldRipples.currentHealth <= 0)
        {
            shieldRipples.currentHealth = 0;
            if(shieldRipples.isTrue)
            {
                shieldRipples.isTrue = false;
                shieldRipples.DestroyShield();
            }
            if(type == E_ChararcterType.player && buff.GetLevel()>=2 && shieldRipples.isTrue)//プレイヤーの盾が壊れた後、ダメージを与える
            {
                shieldRipples.ShieldDamage();
                FMODUnity.RuntimeManager.PlayOneShot("event:/Monster/NorMal/shierdBoom");
                // shieldRipples.isTrue = false;
                // shieldRipples.SetShieldVisble(false);
            }
            // RemoveBuff(buff);
            //シールドバーを破壊する
            // shieldRipples.DestroyShield();
            //表示設定を行う
            // shieldRipples.isTrue = false;
            // shieldRipples.SetShieldVisble(false);
            // shieldRipples = null;
            // FMODUnity.RuntimeManager.PlayOneShot("event:/Monster/NorMal/shierdBoom");         
        }
        return result;
    }

    ///<summary>


    ////// 現在のシールドがまだ有効かどうかを判断します。trueは有効を意味し、falseは無効を意味します。


    ///</summary>
    /// <returns></returns>
    public bool HasShield()
    {
        if(!indexDictionary.ContainsKey(E_BuffKind.ShieldBuff))//シールドがなければ、戻ってください。
        {
            // Debug.Log("シールドがなくなりました");
            return false;
        }
        if(shieldRipples == null)
        {
            // Debug.LogWarning("The buff has already been removed.");
            return false;
        }
        return shieldRipples.isTrue;
    }

    /// <summary>
    /// プレイヤーがコンボを出せる回数を返します
    /// </summary>
    /// <returns></returns>
    public int PlayerSwordTimes()
    {
        if(type != E_ChararcterType.player)
        {
            Debug.LogWarning("Be aware, you are obtaining combo counts for a non-player character.");
            return 0;
        }
        
        if (!indexDictionary.ContainsKey(E_BuffKind.SwordBuff))
        {
            return 2;//現在の企画案では、剣を捨てることはできないため、少なくとも2回の連続攻撃が必要となります。
        }

        SwordBuff buff = indexDictionary[E_BuffKind.SwordBuff] as SwordBuff;
        return buff.GetPlayerTimes();
    }
    /// <summary>
    /// キャラクターの剣のバフレベルを返します
    /// </summary>
    /// <returns></returns>
    public int PlayerSwordLevel()
    {
        if(type != E_ChararcterType.player)
        {
            Debug.LogWarning("Be aware, you are leveling up the sword for a non-player character.");
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
    /// プレイヤーの現在の銃のBuffレベルを返します。0はこのBuffがないことを示します。
    /// </summary>
    /// <returns></returns>
    public int PlayerGunBuffLevel()
    {
        if(type != E_ChararcterType.player)
        {
            Debug.LogWarning("Be aware, you are obtaining firearm Buff levels for a non-player character.");
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
    /// プレイヤーの現在の杖のBuffレベルを返します。0はこのBuffが存在しないことを意味します。
    /// </summary>
    /// <returns></returns>
    public int PlayerStaffBuffLevel()
    {
        if(type != E_ChararcterType.player)
        {
            Debug.LogWarning("Attention, you are obtaining the staff Buff level for a non-player character.");
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
    /// シールド値を回復する
    /// </summary>
    /// <param name="raise">シールドの回復量</param>
    public void RaiseShieldHP(float raise)
    {
        if(!indexDictionary.ContainsKey(E_BuffKind.ShieldBuff))
        {
            Debug.LogWarning("The current character has no shield.");
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
    /// HpItemBuffを追加し、さらなるラッパーを作成します。この関数を使用すると、buffの作成回数を減らすことができます。
    /// </summary>
    /// <param name="chararcterType">キャラクタータイプ</param>
    /// <param name="gameObject">バフ保持者</param>
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
    
    //これは、将来他のアイテムのBuffが存在する可能性に備えたインターフェースで、メソッドはタイプを入力します。辞書にインスタンスが存在しない場合は作成し、存在する場合はAddを再度呼び出します。
    public void AddItemBuff(E_BuffKind buffKind)
    {

    }

}
