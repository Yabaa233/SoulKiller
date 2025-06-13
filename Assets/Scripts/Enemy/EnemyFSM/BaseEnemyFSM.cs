using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 基本敵のFSM
/// </summary>
public class BaseEnemyFSM : MonoBehaviour
{
    public IState currentState;
    public Dictionary<E_EnemyStateType, IState> states = new Dictionary<E_EnemyStateType, IState>();
    public AnimatorStateInfo animatorInfo;//缓存动画信息的位置，这样就不用重复创建了

    /// <summary>
    /// FSMの初期化
    /// </summary>
    public void TranstionState(E_EnemyStateType state)//转换方法
    {
        if (currentState != null)
        {
            currentState.OnExit();  //切换状态先退出当前状态
        }
        currentState = states[state];
        currentState.OnEnter();
    }


    //是否在屏幕内的可见性判断
    public bool IsVisableInCamera{get;private set;}
    private void OnBecameVisible() {
        IsVisableInCamera = true;
    }

    private void OnBecameInvisible() {
        IsVisableInCamera = false;
    }
    
    /// <summary>
    /// FSMの更新
    /// </summary>
    private void Update()
    {
        // 更新逻辑
    }

    /// <summary>
    /// FSMの終了
    /// </summary>
    private void OnDestroy()
    {
        // 清理逻辑
    }
}
