using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowDestroy : MonoBehaviour
{
    [Header("击退力度")]
    public int force;
    [Header("机关伤害")]
    public float attack;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Vector3 forceVector = (other.gameObject.transform.position - gameObject.transform.position) * 1000000000;//这个是专门搞定浮点数误差的
            forceVector[1] = 0;
            if (forceVector == Vector3.zero)
            {
                forceVector = other.transform.forward * 2;
            }
            forceVector = forceVector.normalized;
            other.gameObject.GetComponent<Rigidbody>().AddForce(forceVector * force, ForceMode.Impulse);
            GameManager.Instance.TrickAttackPlayer(AttackPlayer);
        }
        if (other.tag == "EmyBody")
        {
            GameManager.Instance.TrickAttackEnemy(AttackEnemy, other.transform.parent.GetComponent<BaseEnemyControl>());
        }
        if (other.name == "HoldPoint")
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        FMODUnity.RuntimeManager.PlayOneShot("event:/Level/BaoShi/changziGround");
    }
    /// <summary>
    /// 攻击玩家逻辑
    /// </summary>
    /// <param name="curPlayer"> 获取当前玩家 </param>
    private void AttackPlayer(PlayerControl curPlayer)
    {
        if (!curPlayer.characterBuffManager.CalcuSheild(null, attack))
        {
            curPlayer.characterData.currentHealth -= attack;
        }
    }
    /// <summary>
    /// 攻击小怪逻辑
    /// </summary>
    /// <param name="enemy"> 获取当前小怪 </param>
    private float AttackEnemy(BaseEnemyControl enemy)
    {
        if (!enemy.characterBuffManager.CalcuSheild(null, attack))
        {
            enemy.enemyData.currentHealth -= attack;
        }
        return attack;
    }
}
