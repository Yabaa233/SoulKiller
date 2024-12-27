using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

[System.Serializable]
public class CDClass
{
    public float curTime;
    public float maxCDTime;
    public bool flag;
}


public class GameManager : singleton<GameManager>
{
    public Texture2D mousePointer;
    public Texture2D mouseShot;
    [SerializeField]
    public List<CDClass> CDList = new List<CDClass>();
    public List<RoomTrigger> resetRoomList = new List<RoomTrigger>();
    public CDClass comboInterCD = new CDClass();    //连招计数中断CD
    public PlayerControl currentPlayer; //需要玩家代码自己注册
    public BossControl currentBoss; //需要Boss代码自己注册
    public GameObject playerPrefab; //玩家的预制，用于自动生成玩家
    public Transform birthPoint;    //玩家出生点
    private ComboNode currentPlayerNode;    //存储临时ComboNode节点
    private float player_StopTime;    //玩家停止时间
    private bool player_IsStop = false;   //玩家是否正在停止
    private int currentComboCount; //当前连击数
    public int CurrentComboCount { get { return currentComboCount; } }
    public bool Player_IsStop
    {
        get { return player_IsStop; }
    }
    public delegate void TrickAttackPlayerFun(PlayerControl curPlayer);
    public delegate float TrickAttackEnemyFun(BaseEnemyControl curPlayer);
    public delegate float TrickAttackBossFun(BossControl curBoss);
    public delegate void characterStopFun();
    public Action PlayerDie;
    public Action BossDie;
    //游戏通关用时相关
    private float gameStartTime;
    private float gameClearTime;

    protected override void Awake()
    {
        base.Awake();
        SetMouse_Pointer();
        Application.targetFrameRate = 144;   //帧率限制
        DontDestroyOnLoad(this.gameObject);
    }

    public void SetMouse_Pointer()
    {
        Cursor.SetCursor(mousePointer, Vector2.up * 128 + Vector2.right * 512, CursorMode.Auto);
    }
    public void SetMouse_Shot()
    {
        Cursor.SetCursor(mouseShot, Vector2.up * 32 + Vector2.right * 32, CursorMode.Auto);
    }

    private void Start()
    {
        GameManager.Instance.CDList.Add(comboInterCD);
        BossDie += () =>
        {
            gameClearTime = Time.time;
            PanelManager.Instance.Open(new GetBuffPanel());
        };
        comboInterCD.flag = false;
    }
    private void Update()
    {
        CDUpdate();
        //清空连击计数
        if (comboInterCD.flag)
        {
            currentComboCount = 0;
        }
    }

    public void UpadteComboCount()
    {
        comboInterCD.flag = false;
        comboInterCD.curTime = 0;
        currentComboCount++;
    }
    /// <summary>
    /// 玩家开始攻击反馈
    /// </summary>
    public void Player_StartHitEffect()
    {
        player_IsStop = true;

        UpadteComboCount();

        StartCoroutine(Player_HitEffect());
    }

    /// <summary>
    /// 玩家开始射击反馈
    /// </summary>
    public void Player_StartShotEffect(PlayerBullet bullet)
    {
        player_IsStop = true;

        UpadteComboCount();

        StartCoroutine(Player_ShotEffect(bullet));
    }

    /// <summary>
    /// 玩家开始法术攻击反馈
    /// </summary>
    public void Player_StartStaffEffect(PlayerMagic magic)
    {
        player_IsStop = true;

        UpadteComboCount();

        StartCoroutine(Player_StaffEffect(magic));
    }

    /// <summary>
    /// 玩家停顿协程
    /// </summary>
    /// <param name="stopTime"> 停顿时间 </param>
    /// <returns></returns>
    public IEnumerator PlayerStop(float stopTime)
    {
        //测试不使用TimeScale
        // Time.timeScale = 1f;
        EffectManager.Instance.PauseAttackEffect();
        while (stopTime > 0)
        {
            currentPlayer.PauseAnimation(0.1f);
            stopTime -= Time.deltaTime;
            yield return null;
        }
        currentPlayer.ContinueAnimation();
        EffectManager.Instance.PlayAttackEffect();
        yield break;
    }

    /// <summary>
    /// 玩家攻击打击感反馈协程
    /// </summary>
    private IEnumerator Player_HitEffect()
    {
        //打击感——震动
        currentPlayerNode = currentPlayer.currentComboNode;
        CM_Effect.Instance.CM_do_shake(currentPlayerNode.type, currentPlayerNode.shake_time,
                                       currentPlayerNode.amp, currentPlayerNode.fre);
        //打击感——顿帧
        currentPlayer.PauseAnimation(0.3f);
        EffectManager.Instance.PauseAttackEffect();
        while (player_StopTime > 0)
        {
            // Debug.Log(player_StopTime);
            player_StopTime -= Time.deltaTime;
            yield return null;
        }
        currentPlayer.ContinueAnimation();
        EffectManager.Instance.PlayAttackEffect();
        player_IsStop = false;
    }

    /// <summary>
    /// 玩家射击命中感反馈协程
    /// </summary>
    private IEnumerator Player_ShotEffect(PlayerBullet bullet)
    {
        //打击感——震动
        currentPlayerNode = currentPlayer.shotComboNode;
        CM_Effect.Instance.CM_do_shake(currentPlayerNode.type, currentPlayerNode.shake_time,
                                       currentPlayerNode.amp, currentPlayerNode.fre);
        //打击感——顿帧
        currentPlayer.PauseAnimation(0.2f);
        bullet.PlayerBulletStop();
        while (player_StopTime > 0)
        {
            // Debug.Log(player_StopTime);
            player_StopTime -= Time.deltaTime;
            yield return null;
        }
        bullet.PlayerBulletReset();
        currentPlayer.ContinueAnimation();
        player_IsStop = false;
    }

    /// <summary>
    /// 玩家法术命中感反馈协程
    /// </summary>
    private IEnumerator Player_StaffEffect(PlayerMagic magic)
    {
        ParticleSystem magicPartic = magic.migicParticle;
        //打击感——震动
        currentPlayerNode = currentPlayer.magicComboNode;
        CM_Effect.Instance.CM_do_shake(currentPlayerNode.type, currentPlayerNode.shake_time,
                                       currentPlayerNode.amp, currentPlayerNode.fre);
        //打击感——顿帧
        currentPlayer.PauseAnimation(0.3f);
        magicPartic.Pause();
        while (player_StopTime > 0)
        {
            player_StopTime -= Time.deltaTime;
            yield return null;
        }
        magicPartic.Play();
        currentPlayer.ContinueAnimation();
        player_IsStop = false;
    }

    /// <summary>
    /// 玩家的攻击小怪方法
    /// </summary>
    /// <param name="enemy"> 小怪的控制器 </param>
    public void PlayerAttack(BaseEnemyControl enemy, Vector3 attackerPos)
    {
        bool isCritical;
        float damage;
        characterStopFun addStopTime = () => { player_StopTime += player_IsStop ? 0 : enemy.enemyData.currentStopTime; };
        damage = TakeDamage(currentPlayer.characterBuffManager, enemy.characterBuffManager, currentPlayer.characterData, enemy.enemyData, addStopTime, out isCritical);   //计算伤害
        if (attackerPos == Vector3.zero)
        {
            enemy.Damaged(damage, isCritical);
        }
        else
        {
            enemy.Damaged(damage, attackerPos, isCritical);
        }
    }

    /// <summary>
    /// 玩家的攻击Boss方法
    /// </summary>
    /// <param name="boss"> Boss的控制器 </param>
    public void PlayerAttack(BossControl boss)
    {
        if (boss.canGetHit)
        {
            bool isCritical;
            float damage;
            characterStopFun addStopTime = () => { player_StopTime += player_IsStop ? 0 : boss.bossData.currentStopTime; };
            damage = TakeDamage(currentPlayer.characterBuffManager, boss.characterBuffManager, currentPlayer.characterData, boss.bossData, addStopTime, out isCritical);   //计算伤害
            boss.Damaged(damage, isCritical);
            if (currentBoss.lockHealth)
            {
                currentBoss.bossData.currentHealth += damage;
            }
            if (currentBoss.bossData.currentHealth <= 0 && !currentBoss.isDead)
            {
                BossDie();
                return;
            }
        }
    }

    /// <summary>
    /// 敌人的攻击方法
    /// </summary>
    public void EnemyAttack(BaseEnemyControl enemy)
    {
        Color getHitColor = Color.yellow;
        getHitColor.a = 1f;
        if (currentPlayer.animator.GetCurrentAnimatorStateInfo(0).IsTag("Dodge"))
        {
            //角色无敌闪避中
            CM_Effect.Instance.PlayerGetDamaged(getHitColor, 5, 0.8f);
            // Debug.Log("角色无敌，此次伤害无效");
            return;
        }
        bool isCritical;
        float damage = TakeDamage(enemy.characterBuffManager, currentPlayer.characterBuffManager, enemy.enemyData, currentPlayer.characterData, () => { }, out isCritical);
        damage /= 10;
        getHitColor = Color.red;
        getHitColor.a = 0.3f + damage;
        CM_Effect.Instance.PlayerGetDamaged(getHitColor, 5, damage);
        currentPlayer.GetDamage();  //受伤动画
        PlayerHealthCheck();
    }

    /// <summary>
    /// Boss攻击
    /// </summary>
    public void BossAttack()
    {
        Color getHitColor = Color.yellow;
        getHitColor.a = 1f;
        if (currentPlayer.animator.GetCurrentAnimatorStateInfo(0).IsTag("Dodge"))
        {
            //角色无敌闪避中
            CM_Effect.Instance.PlayerGetDamaged(getHitColor, 5, 0.8f);
            // Debug.Log("角色无敌，此次伤害无效");
            return;
        }
        bool isCritical;
        float damage = TakeDamage(currentBoss.characterBuffManager, currentPlayer.characterBuffManager, currentBoss.bossData, currentPlayer.characterData, () => { }, out isCritical);
        damage /= 10;
        getHitColor = Color.red;
        getHitColor.a = 0.4f + damage;
        CM_Effect.Instance.PlayerGetDamaged(getHitColor, 5, damage);
        currentPlayer.GetDamage();  //受伤动画
        PlayerHealthCheck();
    }

    /// <summary>
    /// 机关处理对玩家攻击
    /// </summary>
    /// <param name="trickAttackPlayerFun"> 处理玩家的方法 </param>
    public void TrickAttackPlayer(TrickAttackPlayerFun trickAttackPlayerFun)
    {
        Color getHitColor = Color.yellow;
        getHitColor.a = 1f;
        if (currentPlayer.animator.GetCurrentAnimatorStateInfo(0).IsTag("Dodge"))
        {
            //角色无敌闪避中
            CM_Effect.Instance.PlayerGetDamaged(getHitColor, 5, 0.8f);
            // Debug.Log("角色无敌，此次伤害无效");
            return;
        }
        trickAttackPlayerFun(currentPlayer);
        getHitColor = Color.red;
        getHitColor.a = 1f;
        CM_Effect.Instance.PlayerGetDamaged(getHitColor, 5, 0.8f);
        currentPlayer.GetDamage();
        PlayerHealthCheck();
    }

    /// <summary>
    /// 机关处理对怪物攻击
    /// </summary>
    /// <param name="trickAttackEnemyFun"> 处理怪物的方法 </param>
    public void TrickAttackEnemy(TrickAttackEnemyFun trickAttackEnemyFun, BaseEnemyControl enemy)
    {
        float damage = trickAttackEnemyFun(enemy);
        enemy.Damaged(damage);
    }

    /// <summary>
    /// 机关处理对Boss攻击
    /// </summary>
    /// <param name="trickAttackEnemyFun"> 处理怪物的方法 </param>
    public void TrickAttackBoss(TrickAttackBossFun trickAttackBossFun)
    {
        if (currentBoss.canGetHit)
        {
            float damage = trickAttackBossFun(currentBoss);
            currentBoss.Damaged(damage);
            if (currentBoss.lockHealth)
            {
                currentBoss.bossData.currentHealth += damage;
            }
            if (currentBoss.bossData.currentHealth == 0 && !currentBoss.isDead)
            {
                BossDie();
                return;
            }
        }
    }

    /// <summary>
    /// 伤害判定与伤害计算
    /// 特殊处理
    /// </summary>
    /// <param name="characterBuffManager">攻击方Buff管理器数据</param>
    /// <param name="characterBuffManager">攻击方Buff管理器数据</param>
    /// <param name="attackerData">攻击方数据</param>
    /// <param name="defenderData">防守方数据</param>
    /// <param name="stopFun"></param>
    public float TakeDamage(CharacterBuffManager attackerBuffManager, CharacterBuffManager defenderBuffManager, CharacterData attackerData, CharacterData defenderData, characterStopFun stopFun, out bool isCritical)
    {
        float damage = ResultDamage(attackerData, defenderData, out isCritical);
        if (defenderBuffManager.CalcuSheild(attackerData, damage))    //护盾逻辑
        {
            // Debug.Log("打盾上了");
        }
        else
        {
            stopFun();  //受击自己停顿
            attackerBuffManager.ReturnHP(damage);  //吸血逻辑
            defenderData.currentHealth = Mathf.Max(defenderData.currentHealth - damage, 0f);//防止负血量
        }
        return damage;
    }

    /// <summary>
    /// 计算伤害结果
    /// </summary>
    /// <param name="attackerData"> 攻击方数据 </param>
    /// <param name="defenderData"> 防守方数据 </param>
    /// <returns> 本次伤害值 </returns>
    public float ResultDamage(CharacterData attackerData, CharacterData defenderData, out bool isCritical)
    {
        float ResultDamage = 0f;
        //暴击判断
        if (UnityEngine.Random.Range(0f, 1f) < attackerData.currentCritical)  //暴击
        {
            isCritical = true;
            // Debug.Log("暴击");
            // AkSoundEngine.PostEvent(DefenderData.characterData.getCriticalSound, DefenderData.gameObject);  //播放被暴击音效
            FMODUnity.RuntimeManager.PlayOneShot(defenderData.characterData.getCriticalSound);

            ResultDamage = attackerData.currentComboAttack * (attackerData.currentAttack - defenderData.currentDefend)
                        * (1 + attackerData.currentCriticalDamage);
        }
        else    //没暴击
        {
            isCritical = false;
            //音效 AkSoundEngine.PostEvent(DefenderData.characterData.getHitSound, DefenderData.gameObject);  //播放受击音效
            FMODUnity.RuntimeManager.PlayOneShot(defenderData.characterData.getHitSound);
            ResultDamage = attackerData.currentComboAttack * (attackerData.currentAttack - defenderData.currentDefend);
        }


        //if (attackerData.BaseAttack > 9) DPSManager.Instance.PlusDamage(ResultDamage);//计算玩家DPS
        //if (attackerData.BaseHealth > 1000) DPSManager.Instance.PlusBOSSDamage(ResultDamage);//计算BOSSDPS


        return Mathf.Max(ResultDamage, 0);  //防止回血
    }



    /// <summary>
    /// 更新CD
    /// </summary>
    public void CDUpdate()
    {
        foreach (CDClass temp in CDList)
        {
            if (!temp.flag && temp.curTime < temp.maxCDTime)
            {
                temp.curTime += Time.deltaTime;
                if (temp.curTime > temp.maxCDTime)
                {
                    temp.flag = true;
                    temp.curTime = 0;
                }
            }
        }
    }
    #region 游戏进度管理 暂无本地存储逻辑
    /// <summary>
    /// 游戏正式开始
    /// 进入战斗关卡
    /// </summary>
    public void GameStart()
    {
        PanelManager.Instance.SetMainCamera(Camera.main);   //初始化绑定相机
        birthPoint = GameObject.Find("Level").transform.Find("FirstBirthPoint");    //设置第一个出生点
        //加buff
        BuffDataManager.Instance.playerBuffList.Clear();
        BuffDataManager.Instance.playerBuffList.Add(new SwordBuff(E_ChararcterType.player, 4));
        BuffDataManager.Instance.playerBuffList.Add(new StaffBuff(E_ChararcterType.player, 4));
        BuffDataManager.Instance.playerBuffList.Add(new GunBuff(E_ChararcterType.player, 4));
        BuffDataManager.Instance.playerBuffList.Add(new Damage(E_ChararcterType.player, 4));
        BuffDataManager.Instance.playerBuffList.Add(new HpUp(E_ChararcterType.player, 4));
        BuffDataManager.Instance.playerBuffList.Add(new ShieldBuff(E_ChararcterType.player, 4));
        BuffDataManager.Instance.playerBuffList.Add(new SpeedBuff(E_ChararcterType.player, 4));

        if (currentPlayer != null) Destroy(currentPlayer.gameObject);
        GameObject firstPlayer = Instantiate(playerPrefab, birthPoint.position, playerPrefab.transform.rotation);

        CM_Effect.Instance.SetFollwerPlayer(firstPlayer.transform);     //相机绑定
        firstPlayer.GetComponent<PlayerControl>().lockHealth = true;    //进入后锁血
        gameStartTime = Time.time;
        TimelineManager.Instance.changePlayableTO(0);   //TimeLine操作
        TimelineManager.Instance.PlayCurrentPlayableDirector();   //TimeLine操作
    }
    public void OpenFirstRoomTrigger()
    {
        GameObject.Find("RoomManager").transform.Find("PreBoss").GetComponent<BoxCollider>().enabled = true;
    }

    /// <summary>
    /// 检测玩家是否存活
    /// </summary>
    public void PlayerHealthCheck()
    {
        if (currentPlayer.characterData.currentHealth <= 0)
        {
            currentPlayer.characterData.currentHealth = 0f;
            if (currentPlayer.lockHealth)
            {
                currentPlayer.characterData.currentHealth = 1;
                return;
            }
            else if (currentPlayer.isDead)
            {
                return;
            }
            else
            {
                StartCoroutine(GameOver());
            }
        }
    }

    /// <summary>
    /// 玩家死亡本轮游戏结束
    /// 播放死亡镜头
    /// 恢复镜头
    /// 播放回溯效果
    /// 显示回溯面板
    /// </summary>
    public IEnumerator GameOver()
    {
        float time = 0;

        //通知玩家死亡事件
        Debug.Log("玩家死亡进入GameOver流程");
        PlayerDie();
        while (time < 0.001f)   //执行下一帧
        {
            time += Time.deltaTime;
            yield return null;
        }
        time = 0;

        //镜头拉近 缓速播放死亡动画
        StartCoroutine(PlayerStop(1.0f));
        CM_Effect.Instance.CM_TransitionDim(6, 0.5f);
        while (time < 1.5f)
        {
            time += Time.deltaTime;
            yield return null;
        }
        time = 0;

        //镜头拉远
        CM_Effect.Instance.CM_TransitionDim(18, 1.0f);
        while (time < 1.2f)
        {
            time += Time.deltaTime;
            yield return null;
        }
        time = 0;

        //黑屏等方式掩盖重生效果
        while (time < 3.0f)
        {
            CM_Effect.Instance.SetColorAdjusting(-time * 10);
            time += Time.deltaTime;
            yield return null;
        }
        ResetLevel();
        RemakePlayer();

        //恢复屏幕 恢复玩家输入
        while (time >= 0)
        {
            CM_Effect.Instance.SetColorAdjusting(-time * 10);
            time -= Time.deltaTime;
            yield return null;
        }
        CM_Effect.Instance.SetColorAdjusting(0);
        time = 0;
        currentPlayer.EnableInput();
        PanelManager.Instance.Open(new TipsItem());
        //亮屏 恢复玩家输入控制


        yield break;
    }

    /// <summary>
    /// 通关大罪关卡时调用
    /// </summary>
    public void ClearLevel(RoomTrigger roomTrigger)
    {
        resetRoomList.Clear();
        // var t = JsonUtility.ToJson(resetRoomList);
        // SaveManager.SaveByJson("LevelData", resetRoomList);
        // SaveManager.SaveByJson("ResurrectionPoint", roomTrigger.resurrectionPoint);
        birthPoint = roomTrigger.resurrectionPoint;
        currentPlayer.characterBuffManager.ClearDebuff();
        currentPlayer.characterData.currentHealth = currentPlayer.characterData.maxHealth;

        // BuffDataManager.Instance.RecordBuffList();
    }

    /// <summary>
    /// 死亡时调用
    /// </summary>
    public void ResetLevel()
    {
        // List<RoomTrigger> temp = SaveManager.LoadFromJson<List<RoomTrigger>>("LevelData");
        //重置记录的关卡的状态
        foreach (var t in resetRoomList)
        {
            t.ResetRoom();
        }
        resetRoomList.Clear();
        FmodManager.Instance.stopBGM();
    }

    /// <summary>
    /// 重新创建玩家
    /// </summary>
    public void RemakePlayer()
    {
        //玩家重生
        // birthPoint = SaveManager.LoadFromJson<Transform>("ResurrectionPoint");
        currentPlayer.characterBuffManager.RemoveAllBuff();
        if (currentPlayer != null)
        {
            Destroy(currentPlayer.gameObject);
        }
        //重置Buff
        BuffDataManager.Instance.BackBuff();

        GameObject newPlayer = Instantiate(playerPrefab, birthPoint.position, playerPrefab.transform.rotation);
        currentPlayer = newPlayer.GetComponent<PlayerControl>();
        currentPlayer.DisableInput();
        CM_Effect.Instance.SetFollwerPlayer(newPlayer.transform);   //设置相机跟随玩家

        EffectManager.Instance.SetResurgenceEffect(birthPoint.position);
    }

    /// <summary>
    /// 通关游戏逻辑
    /// </summary>
    public void GameClear()
    {
        currentPlayer.characterBuffManager.RemoveAllBuff();
        //移除Boss和小怪的buff
        PanelManager.Instance.Close(PanelManager.Instance.GetPanel("BattleMainPanel").UIType);
        BuffDataManager.Instance.ClearAllBuff();
        SceneLoadManager.Instance.LoadScene(3);
    }

    /// <summary>
    /// 获取游戏通关流程时间
    /// </summary>
    /// <returns> 通关用秒 </returns>
    public float GetClearTime()
    {
        return gameClearTime - gameStartTime;
    }
    #endregion
}
