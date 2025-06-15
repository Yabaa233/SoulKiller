using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

///<summary>


////// 属性関連のバフ、現在のバフとバフのアップグレード状況を表示します。


///</summary>
public class Buff_Property : Buff_BaseList
{
    static readonly string path = "UI/Item/Buff_Property";

    public Buff_Property() : base(new UIType(path)) { }
}
