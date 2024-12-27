using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceWeapon : MonoBehaviour
{
    private Piece piece;
    private void Awake()
    {
        piece = transform.parent.GetComponent<Piece>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && piece.isAttacking)
        {
            piece.WeaponAttackPlayer();
        }
        if (other.tag == "EnemyBody" && piece.isAttacking)
        {
            piece.WeaponAttackEnemy(other.transform.GetComponent<BaseEnemyControl>());
        }
    }
}
