using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// ������ص�Buff����ʾ��ǰbuff��buff���������
/// xushi
/// </summary>
public class Buff_Weapon : Buff_BaseList
{
    static readonly string path = "UI/Item/Buff_Weapon";

    public Buff_Weapon() : base(new UIType(path)) { }

    
}
