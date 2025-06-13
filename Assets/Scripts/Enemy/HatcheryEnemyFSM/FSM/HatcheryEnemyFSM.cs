using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum E_HatcheryStateType
{
    Idle,//待機
    Production,//生産
    Hit,//被ダメージ
    Dead,//死亡
}
public class HatcheryEnemeyCD
{
    [Header("登録")]
    public CDClass canProduction=new CDClass();
    public void CDInit()
    {
        GameManager.Instance.CDList.Add(canProduction);   //登録
        canProduction.flag = false;
    }
}
[Serializable]
public class HatcheryEnemyParameter
{
    public HatcheryEmyStateData_SO enemyStateData;
    public Animator animator;//アニメーター
    public GameObject _mainCamera;//カメラ
    public CharacterData enemyData;//敵データ
    public Transform body;//本体位置
    public float intervalTime;//インターバル時間
    public bool getHit;
    public bool isDead;
    public Transform sonBorn;//子の生成位置
    public int nowSonCount=0;
}
public class HatcheryEnemyFSM : BaseEnemyFSM
{
    public HatcheryEnemyParameter parameter;
    /*private IState currentState;
    private Dictionary<E_HatcheryStateType, IState> states = new Dictionary<E_HatcheryStateType, IState>();*/
    public CDClass hatcheryEnemyCD=new CDClass();
    private void Start()
    {
        hatcheryEnemyCD.maxCDTime = parameter.intervalTime;
        GameManager.Instance.CDList.Add(hatcheryEnemyCD);
        /*hatcheryEnemyCD.flag = true;*/
        parameter.sonBorn = transform.GetChild(0);
        states.Add(E_EnemyStateType.Idle, new HatcheryEnemy_IdleState(this));
        states.Add(E_EnemyStateType.Attack, new HatcheryEnemy_ProductionState(this));
        states.Add(E_EnemyStateType.Hit, new HatcheryEnemy_HitState(this));
        states.Add(E_EnemyStateType.Dead, new HatcheryEnemy_DeadState(this));
        TranstionState(E_EnemyStateType.Idle);//初期状態をIdleに設定
    }
    private void Update()//状態更新と状態遷移の実行
    {
        FaceToCamera();
        currentState.OnUpDate();
    }
    public void FaceToCamera()
    {
        var rotation = Quaternion.LookRotation(parameter._mainCamera.transform.TransformVector(Vector3.forward),
            parameter._mainCamera.transform.TransformVector(Vector3.up));
        rotation = new Quaternion(0, rotation.y, 0, rotation.w);
        gameObject.transform.rotation = rotation;
    }
    public GameObject SonProduction(Transform Position)
    {
        return Instantiate(parameter.enemyStateData.sonPrefab,Position.position,Position.rotation);
    }
}
