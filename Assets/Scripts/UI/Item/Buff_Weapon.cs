using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 武器相关的Buff，显示当前buff和buff的升级情况
/// xushi
/// </summary>
public class Buff_Weapon : Buff_BaseList
{
    static readonly string path = "UI/Item/Buff_Weapon";

    public Buff_Weapon() : base(new UIType(path)) { }

    
}
