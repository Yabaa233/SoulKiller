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
    public CDClass comboInterCD = new CDClass();    //連続技カウント中断CD
    public PlayerControl currentPlayer; //プレイヤーは自分でコードを登録する必要があります。
    public BossControl currentBoss; //ボスコードを自分で登録する必要があります
    public GameObject playerPrefab; //プレイヤーのプレファブ、自動的にプレイヤーを生成するために使用されます。
    public Transform birthPoint;    //プレイヤーのスポーンポイント
    private ComboNode currentPlayerNode;    //一時的なComboNodeノードを保存します
    private float player_StopTime;    //プレイヤーは時間を停止します
    private bool player_IsStop = false;   //プレイヤーは停止中ですか？
    private int currentComboCount; //現在のコンボ数
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
    //ゲームクリアに関する所要時間
    private float gameStartTime;
    private float gameClearTime;

    protected override void Awake()
    {
        base.Awake();
        SetMouse_Pointer();
        Application.targetFrameRate = 144;   //フレームレート制限
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
        //コンボカウントをリセット
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
    /// プレイヤーはフィードバックを攻撃し始めました。
    /// </summary>
    public void Player_StartHitEffect()
    {
        player_IsStop = true;

        UpadteComboCount();

        StartCoroutine(Player_HitEffect());
    }

    /// <summary>
    /// プレイヤーがシューティングフィードバックを開始します
    /// </summary>
    public void Player_StartShotEffect(PlayerBullet bullet)
    {
        player_IsStop = true;

        UpadteComboCount();

        StartCoroutine(Player_ShotEffect(bullet));
    }

    /// <summary>
    /// プレイヤーが魔法攻撃のフィードバックを開始します
    /// </summary>
    public void Player_StartStaffEffect(PlayerMagic magic)
    {
        player_IsStop = true;

        UpadteComboCount();

        StartCoroutine(Player_StaffEffect(magic));
    }

    /// <summary>
    /// プレイヤーがコルーチンを一時停止
    /// </summary>
    /// <param name="stopTime"> 停止時間 </param>
    /// <returns></returns>
    public IEnumerator PlayerStop(float stopTime)
    {
        //TimeScaleを使用しないテスト
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
    /// プレイヤーの攻撃ヒット感フィードバックのコルーチン
    /// </summary>
    private IEnumerator Player_HitEffect()
    {
        //打撃感 - 振動
        currentPlayerNode = currentPlayer.currentComboNode;
        CM_Effect.Instance.CM_do_shake(currentPlayerNode.type, currentPlayerNode.shake_time,
                                       currentPlayerNode.amp, currentPlayerNode.fre);
        //打撃感 - フレーム停止
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
    /// プレイヤーのショットヒット感のフィードバックコルーチン
    /// </summary>
    private IEnumerator Player_ShotEffect(PlayerBullet bullet)
    {
        //打撃感 - 振動
        currentPlayerNode = currentPlayer.shotComboNode;
        CM_Effect.Instance.CM_do_shake(currentPlayerNode.type, currentPlayerNode.shake_time,
                                       currentPlayerNode.amp, currentPlayerNode.fre);
        //打撃感 - フレーム停止
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
    /// プレイヤーのスペルヒット感のフィードバックコルーチン
    /// </summary>
    private IEnumerator Player_StaffEffect(PlayerMagic magic)
    {
        ParticleSystem magicPartic = magic.migicParticle;
        //打撃感 - 振動
        currentPlayerNode = currentPlayer.magicComboNode;
        CM_Effect.Instance.CM_do_shake(currentPlayerNode.type, currentPlayerNode.shake_time,
                                       currentPlayerNode.amp, currentPlayerNode.fre);
        //打撃感 - フレーム停止
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
    /// プレイヤーのモンスター攻撃方法
    /// </summary>
    /// <param name="enemy"> モンスターのコントローラー </param>
    public void PlayerAttack(BaseEnemyControl enemy, Vector3 attackerPos)
    {
        bool isCritical;
        float damage;
        characterStopFun addStopTime = () => { player_StopTime += player_IsStop ? 0 : enemy.enemyData.currentStopTime; };
        damage = TakeDamage(currentPlayer.characterBuffManager, enemy.characterBuffManager, currentPlayer.characterData, enemy.enemyData, addStopTime, out isCritical);   //ダメージを計算する
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
    /// プレイヤーのボス攻撃方法
    /// </summary>
    /// <param name="boss"> ボスのコントローラー </param>
    public void PlayerAttack(BossControl boss)
    {
        if (boss.canGetHit)
        {
            bool isCritical;
            float damage;
            characterStopFun addStopTime = () => { player_StopTime += player_IsStop ? 0 : boss.bossData.currentStopTime; };
            damage = TakeDamage(currentPlayer.characterBuffManager, boss.characterBuffManager, currentPlayer.characterData, boss.bossData, addStopTime, out isCritical);   //ダメージを計算する
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

    ///<summary>


    ////// 敵の攻撃方法


    ///</summary>
    public void EnemyAttack(BaseEnemyControl enemy)
    {
        Color getHitColor = Color.yellow;
        getHitColor.a = 1f;
        if (currentPlayer.animator.GetCurrentAnimatorStateInfo(0).IsTag("Dodge"))
        {
            //キャラクターは無敵回避中です。
            CM_Effect.Instance.PlayerGetDamaged(getHitColor, 5, 0.8f);
            // Debug.Log("キャラクターは無敵で、このダメージは無効です。");
            return;
        }
        bool isCritical;
        float damage = TakeDamage(enemy.characterBuffManager, currentPlayer.characterBuffManager, enemy.enemyData, currentPlayer.characterData, () => { }, out isCritical);
        damage /= 10;
        getHitColor = Color.red;
        getHitColor.a = 0.3f + damage;
        CM_Effect.Instance.PlayerGetDamaged(getHitColor, 5, damage);
        currentPlayer.GetDamage();  //怪我アニメーション
        PlayerHealthCheck();
    }

    /// <summary>
    /// ボスが攻撃します
    /// </summary>
    public void BossAttack()
    {
        Color getHitColor = Color.yellow;
        getHitColor.a = 1f;
        if (currentPlayer.animator.GetCurrentAnimatorStateInfo(0).IsTag("Dodge"))
        {
            //キャラクターは無敵回避中です。
            CM_Effect.Instance.PlayerGetDamaged(getHitColor, 5, 0.8f);
            // Debug.Log("キャラクターは無敵で、このダメージは無効です。");
            return;
        }
        bool isCritical;
        float damage = TakeDamage(currentBoss.characterBuffManager, currentPlayer.characterBuffManager, currentBoss.bossData, currentPlayer.characterData, () => { }, out isCritical);
        damage /= 10;
        getHitColor = Color.red;
        getHitColor.a = 0.4f + damage;
        CM_Effect.Instance.PlayerGetDamaged(getHitColor, 5, damage);
        currentPlayer.GetDamage();  //怪我アニメーション
        PlayerHealthCheck();
    }

    ///<summary>


    ////// 機関はプレイヤーに攻撃を行います


    ///</summary>
    /// <param name="trickAttackPlayerFun"> プレイヤーの処理方法 </param>
    public void TrickAttackPlayer(TrickAttackPlayerFun trickAttackPlayerFun)
    {
        Color getHitColor = Color.yellow;
        getHitColor.a = 1f;
        if (currentPlayer.animator.GetCurrentAnimatorStateInfo(0).IsTag("Dodge"))
        {
            //キャラクターは無敵回避中です。
            CM_Effect.Instance.PlayerGetDamaged(getHitColor, 5, 0.8f);
            // Debug.Log("キャラクターは無敵で、このダメージは無効です。");
            return;
        }
        trickAttackPlayerFun(currentPlayer);
        getHitColor = Color.red;
        getHitColor.a = 1f;
        CM_Effect.Instance.PlayerGetDamaged(getHitColor, 5, 0.8f);
        currentPlayer.GetDamage();
        PlayerHealthCheck();
    }

    ///<summary>


    ////// 機関はモンスターの攻撃を処理します。


    ///</summary>
    /// <param name="trickAttackEnemyFun"> モンスターの処理方法 </param>
    public void TrickAttackEnemy(TrickAttackEnemyFun trickAttackEnemyFun, BaseEnemyControl enemy)
    {
        float damage = trickAttackEnemyFun(enemy);
        enemy.Damaged(damage);
    }

    ///<summary>


    ////// 機関はボスへの攻撃を処理します


    ///</summary>
    /// <param name="trickAttackEnemyFun"> モンスターの処理方法 </param>
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
    /// ダメージ判定とダメージ計算
    /// 特別処理
    /// </summary>
    /// <param name="characterBuffManager">攻撃者のBuff管理器データ</param>
    /// <param name="characterBuffManager">攻撃者のBuff管理器データ</param>
    /// <param name="attackerData">攻撃者のデータ</param>
    /// <param name="defenderData">防衛者データ</param>
    /// <param name="stopFun"></param>
    public float TakeDamage(CharacterBuffManager attackerBuffManager, CharacterBuffManager defenderBuffManager, CharacterData attackerData, CharacterData defenderData, characterStopFun stopFun, out bool isCritical)
    {
        float damage = ResultDamage(attackerData, defenderData, out isCritical);
        if (defenderBuffManager.CalcuSheild(attackerData, damage))    //シールドロジック
        {
            // Debug.Log("シールドがヒットされた");
        }
        else
        {
            stopFun();  //自分自身が打撃を受けて停止する
            attackerBuffManager.ReturnHP(damage);  //吸血論理
            defenderData.currentHealth = Mathf.Max(defenderData.currentHealth - damage, 0f);//マイナス血液量を防ぐ
        }
        return damage;
    }

    /// <summary>
    /// ダメージ結果を計算する
    /// </summary>
    /// <param name="attackerData"> 攻撃者のデータ </param>
    /// <param name="defenderData"> 防御側のデータ </param>
    /// <returns> 今回のダメージ値 </returns>
    public float ResultDamage(CharacterData attackerData, CharacterData defenderData, out bool isCritical)
    {
        float ResultDamage = 0f;
        //クリティカル判定
        if (UnityEngine.Random.Range(0f, 1f) < attackerData.currentCritical)  //クリティカルヒット
        {
            isCritical = true;
            // Debug.Log("クリティカルヒット");
            // AkSoundEngine.PostEvent(DefenderData.characterData.getCriticalSound, DefenderData.gameObject);  //クリティカルヒットの音を再生する
            FMODUnity.RuntimeManager.PlayOneShot(defenderData.characterData.getCriticalSound);

            ResultDamage = attackerData.currentComboAttack * (attackerData.currentAttack - defenderData.currentDefend)
                        * (1 + attackerData.currentCriticalDamage);
        }
        else    //クリティカルヒットがない
        {
            isCritical = false;
            //サウンドエフェクト AkSoundEngine.PostEvent(DefenderData.characterData.getHitSound, DefenderData.gameObject);  // ヒットサウンドを再生
            FMODUnity.RuntimeManager.PlayOneShot(defenderData.characterData.getHitSound);
            ResultDamage = attackerData.currentComboAttack * (attackerData.currentAttack - defenderData.currentDefend);
        }


        //if (attackerData.BaseAttack > 9) DPSManager.Instance.PlusDamage(ResultDamage);//プレイヤーのDPSを計算する
        //if (attackerData.BaseHealth > 1000) DPSManager.Instance.PlusBOSSDamage(ResultDamage);//BOSSDPSを計算する


        return Mathf.Max(ResultDamage, 0);  //回復を防ぐ
    }



    /// <summary>
    /// CDを更新する
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
    /// ゲームが正式に始まりました
    /// バトルステージに入る
    /// </summary>
    public void GameStart()
    {
        PanelManager.Instance.SetMainCamera(Camera.main);   //カメラの初期化バインド
        birthPoint = GameObject.Find("Level").transform.Find("FirstBirthPoint");    //最初のスポーンポイントを設定する
        //バフを追加する
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

        CM_Effect.Instance.SetFollwerPlayer(firstPlayer.transform);     //カメラバインド
        firstPlayer.GetComponent<PlayerControl>().lockHealth = true;    //後に入ってから血をロックします
        gameStartTime = Time.time;
        TimelineManager.Instance.changePlayableTO(0);   //タイムライン操作
        TimelineManager.Instance.PlayCurrentPlayableDirector();   //タイムライン操作
    }
    public void OpenFirstRoomTrigger()
    {
        GameObject.Find("RoomManager").transform.Find("PreBoss").GetComponent<BoxCollider>().enabled = true;
    }

    /// <summary>
    /// プレイヤーが生存しているかどうかを確認する
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
    /// プレイヤーが死亡したら、このラウンドのゲームは終了します。
    /// デスシーンを再生する
    /// レンズを復元する
    /// 再生リトラクション効果
    /// バックトレースパネルを表示する
    /// </summary>
    public IEnumerator GameOver()
    {
        float time = 0;

        //プレイヤーの死亡イベントを通知する
        Debug.Log("プレイヤーが死亡し、ゲームオーバーのプロセスに入ります。");
        PlayerDie();
        while (time < 0.001f)   //次のフレームを実行する
        {
            time += Time.deltaTime;
            yield return null;
        }
        time = 0;

        //カメラがズームインし、死亡アニメーションがスローモーションで再生されます。
        StartCoroutine(PlayerStop(1.0f));
        CM_Effect.Instance.CM_TransitionDim(6, 0.5f);
        while (time < 1.5f)
        {
            time += Time.deltaTime;
            yield return null;
        }
        time = 0;

        //カメラが引きます
        CM_Effect.Instance.CM_TransitionDim(18, 1.0f);
        while (time < 1.2f)
        {
            time += Time.deltaTime;
            yield return null;
        }
        time = 0;

        //黒い画面などでリスポーン効果を隠す
        while (time < 3.0f)
        {
            CM_Effect.Instance.SetColorAdjusting(-time * 10);
            time += Time.deltaTime;
            yield return null;
        }
        ResetLevel();
        RemakePlayer();

        //画面を復元する プレイヤーの入力を復元する
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
        //画面を明るくし、プレイヤーの入力制御を回復します


        yield break;
    }

    ///<summary>


    ////// 大罪のステージをクリアした時に呼び出す


    ///</summary>
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

    ///<summary>


    ////// 死亡時に呼び出す


    ///</summary>
    public void ResetLevel()
    {
        // List<RoomTrigger> temp = SaveManager.LoadFromJson<List<RoomTrigger>>("LevelData");
        //レコードのステージの状態をリセットする
        foreach (var t in resetRoomList)
        {
            t.ResetRoom();
        }
        resetRoomList.Clear();
        FmodManager.Instance.stopBGM();
    }

    /// <summary>
    /// プレイヤーを再作成する
    /// </summary>
    public void RemakePlayer()
    {
        //プレイヤーのリスポーン
        // birthPoint = SaveManager.LoadFromJson<Transform>("ResurrectionPoint");
        currentPlayer.characterBuffManager.RemoveAllBuff();
        if (currentPlayer != null)
        {
            Destroy(currentPlayer.gameObject);
        }
        //バフをリセット
        BuffDataManager.Instance.BackBuff();

        GameObject newPlayer = Instantiate(playerPrefab, birthPoint.position, playerPrefab.transform.rotation);
        currentPlayer = newPlayer.GetComponent<PlayerControl>();
        currentPlayer.DisableInput();
        CM_Effect.Instance.SetFollwerPlayer(newPlayer.transform);   //カメラをプレイヤーに追従させる設定

        EffectManager.Instance.SetResurgenceEffect(birthPoint.position);
    }

    /// <summary>
    /// ゲームのロジックをクリアする
    /// </summary>
    public void GameClear()
    {
        currentPlayer.characterBuffManager.RemoveAllBuff();
        //ボスとモンスターのバフを削除する
        PanelManager.Instance.Close(PanelManager.Instance.GetPanel("BattleMainPanel").UIType);
        BuffDataManager.Instance.ClearAllBuff();
        SceneLoadManager.Instance.LoadScene(3);
    }

    /// <summary>
    /// ゲームクリアのプロセス時間を取得する
    /// </summary>
    /// <returns> クリア時間は秒 </returns>
    public float GetClearTime()
    {
        return gameClearTime - gameStartTime;
    }
    #endregion
}
