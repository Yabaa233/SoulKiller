using UnityEngine;

[System.Serializable]
public enum E_BuffKind //あなたはどの種類のバフに属していますか？
{
    HpUp = 4,
    Damage = 2,
    SpeedBuff = 1,
    ShieldBuff = 3,
    SwordBuff = 0,
    GunBuff = 6,
    StaffBuff = 5,
    HpItemBuff = 7,
}

public interface I_BuffBase
{
    /// <summary>
    /// エンティティを追加する際のロジック
    /// </summary>
    public void OnAdd(GameObject _buffKeeper);
        
    /// <summary>
    /// それぞれのエンティティフレームに従って更新します。
    /// </summary>
    public void OnUpdate(float deltaTime);//時間変数が必要です

    ///<summary>
    ////// エンティティが削除されるとき
    ///</summary>
    public void OnRemove();
    /// <summary>
    /// あなたはどの種類のバフを得ましたか？
    /// </summary>
    /// <returns></returns>
    public E_BuffKind GetBuffType();
    /// <summary>
    /// バフを得た所有者
    /// </summary>
    /// <returns></returns>
    public GameObject GetBuffKeeper();
    ///<summary>
    ////// 現在の役割タイプを取得する
    ///</summary>
    /// <returns></returns>
    public E_ChararcterType GetChararcterType();
    ///<summary>
    ////// 現在のBuffのレベルを取得する
    ///</summary>
    /// <returns></returns>
    public int GetLevel();
}
