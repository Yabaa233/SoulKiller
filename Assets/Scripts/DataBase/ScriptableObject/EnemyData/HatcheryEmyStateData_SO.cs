using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObject/Battle/HatcheryEmyStateData")]
public class HatcheryEmyStateData_SO : ScriptableObject
{
    [Header("モンスターのプレハブを生成する")]
    public GameObject sonPrefab;
    [Header("モンスター生成の上限")]
    public int sonMaxCount;
}
