using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 属性相关的Buff，显示当前buff和buff的升级情况
/// xushi
/// </summary>
public class Buff_Property : Buff_BaseList
{
    static readonly string path = "UI/Item/Buff_Property";

    public Buff_Property() : base(new UIType(path)) { }
}
