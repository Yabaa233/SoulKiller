using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowDestroy : MonoBehaviour
{
    [Header("Repelling force")]
    public int force;
    [Header("Institutional harm")]
    public float attack;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Vector3 forceVector = (other.gameObject.transform.position - gameObject.transform.position) * 1000000000;//これは浮動小数点の誤差を専門的に解決するためのものです。
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
    /// プレイヤーの攻撃ロジック
    /// </summary>
    /// <param name="curPlayer"> 現在のプレイヤーを取得 </param>
    private void AttackPlayer(PlayerControl curPlayer)
    {
        if (!curPlayer.characterBuffManager.CalcuSheild(null, attack))
        {
            curPlayer.characterData.currentHealth -= attack;
        }
    }
    /// <summary>
    /// モンスター攻撃ロジック
    /// </summary>
    /// <param name="enemy"> 現在のモンスターを取得 </param>
    private float AttackEnemy(BaseEnemyControl enemy)
    {
        if (!enemy.characterBuffManager.CalcuSheild(null, attack))
        {
            enemy.enemyData.currentHealth -= attack;
        }
        return attack;
    }
}
