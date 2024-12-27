using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpItem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player")
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Player/heal");
            PlayerControl playerControl = other.gameObject.GetComponent<PlayerControl>();
            playerControl.characterBuffManager.AddHpItemBuff(E_ChararcterType.player,other.gameObject);
            Destroy(this.gameObject);
        }
    }
}
