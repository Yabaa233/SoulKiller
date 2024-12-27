using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FlowerMonsterControl : MonoBehaviour
{
    [Header("间隔时间")]
    public float intervalTime;
    public Transform target;
    [Header("大花光炮与死亡事件")]
    public GameObject radio;
    private float accumulateTime;
    public bool trapStart, isBoss, isDead;
    public GameObject deadEffect;
    // Start is called before the first frame update
    void Start()
    {
        accumulateTime = 0;
        target = GameManager.Instance.currentPlayer.transform;
    }
    private void OnEnable()
    {
        transform.parent.parent.GetComponent<TrapTrigger>().openTarp += () => trapStart = true;
        radio.GetComponent<FlowerRotate>().broadcastStart += new UnityAction(CantAttack);
        radio.GetComponent<FlowerRotate>().broadcastEnd += new UnityAction(CanAttack);
        transform.parent.GetComponent<EnvyColumn>().Rely += new UnityAction(Dead);
    }
    private void OnDisable()
    {
        radio.GetComponent<FlowerRotate>().broadcastStart -= CantAttack;
        radio.GetComponent<FlowerRotate>().broadcastEnd -= CanAttack;
        transform.parent.GetComponent<EnvyColumn>().Rely -= Dead;
    }
    // Update is called once per frame
    void Update()
    {
        if (trapStart)
        {
            if (!isDead)
            {
                if (!isBoss)
                {
                    transform.LookAt(new Vector3(
                        target.position.x,
                        transform.position.y,
                        target.position.z));
                    Vector3 newEuler = transform.rotation.eulerAngles + new Vector3(0f, 90f, -90f);
                    transform.rotation = Quaternion.Euler(newEuler);
                    accumulateTime += Time.deltaTime;
                    if (accumulateTime >= intervalTime)
                    {
                        if (gameObject != null)
                        {
                            GameObject bullet = ObjectPool.Instance.GetObject("FlowerBullet", EffectManager.Instance.transform);
                            bullet.transform.position = gameObject.transform.position;
                            bullet.GetComponent<FlowerBullet>().Shot(target.position);
                            bullet.SetActive(true);
                        }
                        accumulateTime = 0;
                    }
                }
            }
        }
    }
    void CanAttack()
    {
        isBoss = false;
    }
    void CantAttack()
    {
        isBoss = true;
    }
    void Dead()
    {
        Instantiate(deadEffect, transform.position, transform.rotation);
        isDead = true;
        Destroy(gameObject.transform.parent.gameObject, 2f);
    }
}
