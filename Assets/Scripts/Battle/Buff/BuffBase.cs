using UnityEngine;

[System.Serializable]
public enum E_BuffKind //属于哪一种Buff
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
    /// 添加到实体的时候的逻辑
    /// </summary>
    public void OnAdd(GameObject _buffKeeper);
        
    /// <summary>
    /// 跟随实体每一帧进行更新
    /// </summary>
    public void OnUpdate(float deltaTime);//需要时间变量
    /// <summary>
    /// 当从实体移除时
    /// </summary>
    public void OnRemove();
    /// <summary>
    /// 得到是哪种Buff
    /// </summary>
    /// <returns></returns>
    public E_BuffKind GetBuffType();
    /// <summary>
    /// 得到Buff的持有者
    /// </summary>
    /// <returns></returns>
    public GameObject GetBuffKeeper();
    /// <summary>
    /// 得到当前角色类型
    /// </summary>
    /// <returns></returns>
    public E_ChararcterType GetChararcterType();
    /// <summary>
    /// 得到当前Buff的等级
    /// </summary>
    /// <returns></returns>
    public int GetLevel();

}
