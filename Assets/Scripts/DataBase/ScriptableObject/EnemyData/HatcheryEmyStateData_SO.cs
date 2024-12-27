using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObject/Battle/HatcheryEmyStateData")]
public class HatcheryEmyStateData_SO : ScriptableObject
{
    [Header("生成小怪物的prefab")]
    public GameObject sonPrefab;
    [Header("生成小怪物的上限")]
    public int sonMaxCount;
}
