using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class DamageMassif : MonoBehaviour
{
    //TODO:火焰地块伤害结算频次
    [Header("�����˺�")]
    public float attack;
    public GameObject coreGameObject;
    public List<GameObject> effect;
    public bool trapStart;
    public float intervalTime, continueTime;
    private float startTime, endTime;
    private bool isOpen;
    private void OnEnable()
    {
        transform.parent.GetComponent<TrapTrigger>().openTarp += () => trapStart = true;
        coreGameObject.GetComponent<SpitFireControl>().DeathNotice += MassifClose;
        endTime = Time.time;
    }
    private void FixedUpdate()
    {
        if (trapStart)
        {
            if (!isOpen && Time.time > endTime + intervalTime)
            {
                isOpen = true;
                startTime = Time.time;
                gameObject.GetComponent<Collider>().enabled = true;
            }
            else
            {
                if (isOpen && Time.time > startTime + continueTime)
                {
                    gameObject.GetComponent<Collider>().enabled = false;
                    endTime = Time.time;
                    isOpen = false;
                }

            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (trapStart)
        {
            if (other.tag == "Player")
            {
                var curPlayer = GameManager.Instance.currentPlayer;
                if (curPlayer.animator.GetCurrentAnimatorStateInfo(0).IsTag("Dodge"))
                {
                    return;
                }
                if (curPlayer != null)
                {
                    if (!curPlayer.characterBuffManager.CalcuSheild(null, attack))
                    {
                        curPlayer.characterData.currentHealth -= attack;
                    }
                    GameManager.Instance.PlayerHealthCheck();
                }
            }
            if (other.tag == "EmyBody")
            {
                GameManager.Instance.TrickAttackEnemy(AttackEnemy, other.transform.parent.GetComponent<BaseEnemyControl>());
            }
        }
    }
    private void OnDisable()
    {
        coreGameObject.GetComponent<SpitFireControl>().DeathNotice -= MassifClose;
    }
    /// <summary>
    /// ����С���߼�
    /// </summary>
    /// <param name="enemy"> ��ȡ��ǰС�� </param>
    private float AttackEnemy(BaseEnemyControl enemy)
    {
        if (!enemy.characterBuffManager.CalcuSheild(null, attack))
        {
            enemy.enemyData.currentHealth -= attack;
        }
        return attack;
    }
    private void MassifClose()
    {
        this.trapStart = false;
        foreach (var i in effect)
        {
            i.SetActive(false);
        }
        gameObject.SetActive(false);
    }
}
