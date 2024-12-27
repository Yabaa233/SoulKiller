using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneJump : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            GameObject.Find("PanelManager").transform.Find("UIRoot").Find("Canvas").Find("BattleUI").Find("SkipButton").gameObject.SetActive(false);
            SceneLoadManager.Instance.LoadBattleScene(2);
        }
    }
}
