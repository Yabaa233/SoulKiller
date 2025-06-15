using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>


////// 基本的な敵のFSM


///</summary>
public class BaseEnemyFSM : MonoBehaviour
{
    public IState currentState;
    public Dictionary<E_EnemyStateType, IState> states = new Dictionary<E_EnemyStateType, IState>();
    public AnimatorStateInfo animatorInfo;//アニメーション情報をキャッシュする位置、これにより再作成の必要がなくなります。

    /// <summary>
    /// FSMの初期化
    /// </summary>
    public void TranstionState(E_EnemyStateType state)//変換方法
    {
        if (currentState != null)
        {
            currentState.OnExit();  //ステータスを切り替える前に、現在のステータスを終了してください。
        }
        currentState = states[state];
        currentState.OnEnter();
    }


    //画面内の可視性の判断
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
        // ロジックの更新
    }

    /// <summary>
    /// FSMの終了
    /// </summary>
    private void OnDestroy()
    {
        // ロジックのクリーニング
    }
}
