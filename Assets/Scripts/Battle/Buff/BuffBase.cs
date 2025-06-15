using UnityEngine;

[System.Serializable]
public enum E_BuffKind //どの種類のバフに属していますか？
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
    /// エンティティに追加する際のロジック
    /// </summary>
    public void OnAdd(GameObject _buffKeeper);
        
    /// <summary>
    /// エンティティの各フレームに従って更新します
    /// </summary>
    public void OnUpdate(float deltaTime);//時間変数が必要です

    ///<summary>
    ////// 実体が削除されるとき
    ///</summary>
    public void OnRemove();
    /// <summary>
    /// どの種類のバフを得ましたか？
    /// </summary>
    /// <returns></returns>
    public E_BuffKind GetBuffType();
    /// <summary>
    /// バフを得た持ち主
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
