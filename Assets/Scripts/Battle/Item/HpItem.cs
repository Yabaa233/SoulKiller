using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HPアイテム
/// </summary>
public class HpItem : MonoBehaviour
{
    /// <summary>
    /// アイテムの初期化
    /// </summary>
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player")
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Player/heal");
            PlayerControl playerControl = other.gameObject.GetComponent<PlayerControl>();
            playerControl.characterBuffManager.AddHpItemBuff(E_ChararcterType.player,other.gameObject);
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// アイテムの更新
    /// </summary>
    // ... existing code ...

    /// <summary>
    /// アイテムの終了
    /// </summary>
    // ... existing code ...
}
