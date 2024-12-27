using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public bool isTouch;
    public float touchTime;
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Knife")
        {
            other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.SetActive(false);
            isTouch = true;
            touchTime = Time.time;
        }
    }
}
