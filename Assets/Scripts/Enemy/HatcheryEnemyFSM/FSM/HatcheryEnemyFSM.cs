using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum E_HatcheryStateType
{
    Idle,//��ֹ
    Production,//����
    Hit,//�ܻ�
    Dead,//����
}
public class HatcheryEnemeyCD
{
    [Header("�������")]
    public CDClass canProduction=new CDClass();
    public void CDInit()
    {
        GameManager.Instance.CDList.Add(canProduction);   //�չ�
        canProduction.flag = false;
    }
}
[Serializable]
public class HatcheryEnemyParameter
{
    public HatcheryEmyStateData_SO enemyStateData;
    public Animator animator;//����������
    public GameObject _mainCamera;//���
    public CharacterData enemyData;//��������
    public Transform body;//����λ��
    public float intervalTime;//�������ʱ��
    public bool getHit;
    public bool isDead;
    public Transform sonBorn;//С�ֳ�����
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
        TranstionState(E_EnemyStateType.Idle);//��ʼ��״̬ΪIdle
    }
    private void Update()//ִ��״̬����״̬��������
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
