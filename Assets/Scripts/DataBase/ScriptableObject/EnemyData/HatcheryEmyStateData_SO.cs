using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObject/Battle/HatcheryEmyStateData")]
public class HatcheryEmyStateData_SO : ScriptableObject
{
    [Header("����С�����prefab")]
    public GameObject sonPrefab;
    [Header("����С���������")]
    public int sonMaxCount;
}
