using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    public Text damageText;//伤害数字
    public float lifeTimer;//存在多久
    public float upSpeed;//上升速度
    void Start()
    {
        damageText = gameObject.GetComponent<Text>();
        Invoke("RecycleObj",2);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0,upSpeed * Time.deltaTime,0);
        // damageText.text = "100";
    }

    public void SetDamage(float _damage)
    {
        damageText.text = _damage.ToString();
    }

    public void SetCritical(bool state)
    {   

    }


    public void RecycleObj()
    {
        ObjectPool.Instance.RecycleObj("DamageText", gameObject);
    }
}
