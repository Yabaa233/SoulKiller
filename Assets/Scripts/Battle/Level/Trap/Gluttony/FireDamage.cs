using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireDamage : MonoBehaviour
{
    [Header("�����˺�")]
    public float attack;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameManager.Instance.TrickAttackPlayer(AttackPlayer);
        }
        if (other.tag == "EmyBody")
        {
            GameManager.Instance.TrickAttackEnemy(AttackEnemy, other.transform.parent.GetComponent<BaseEnemyControl>());
        }
    }
    /// <summary>
    /// ��������߼�
    /// </summary>
    /// <param name="curPlayer"> ��ȡ��ǰ��� </param>
    private void AttackPlayer(PlayerControl curPlayer)
    {
        if (!curPlayer.characterBuffManager.CalcuSheild(null, attack))
        {
            curPlayer.characterData.currentHealth -= attack;
        }
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
}
