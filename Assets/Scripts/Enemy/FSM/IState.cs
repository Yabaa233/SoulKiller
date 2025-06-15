///<summary>
////// 敵の状態インターフェース
///</summary>
public interface IState
{
    ///<summary>

    ////// 状態の初期化

    ///</summary>
    void OnEnter();
    ///<summary>

    ////// 状態の更新

    ///</summary>
    void OnUpDate();
    void OnLateUpDade();
    /// <summary>
    /// ステータスの終了
    /// </summary>
    void OnExit();
}