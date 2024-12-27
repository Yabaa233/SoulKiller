using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class FoodDestroy : MonoBehaviour
{
    public RoomTrigger roomTrigger;
    public UnityAction eliminate;
    private void Awake()
    {
        roomTrigger = transform.parent.parent.GetComponent<RoomTrigger>();
    }
    private void OnEnable()
    {
        roomTrigger.ClearScenc += GameOver;
    }
    private void OnDisable()
    {
        roomTrigger.ClearScenc -= GameOver;
    }
    void GameOver()
    {
        if (eliminate != null)
        {
            eliminate();
        }
        Destroy(gameObject);
    }
}
