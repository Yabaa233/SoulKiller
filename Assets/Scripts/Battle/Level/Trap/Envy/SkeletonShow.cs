using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SkeletonShow : MonoBehaviour
{
    public void OpenEyes()
    {
        transform.GetChild(3).gameObject.SetActive(true);
        transform.GetChild(4).gameObject.SetActive(true);
    }
    public void CloseEyes()
    {
        transform.GetChild(3).gameObject.SetActive(false);
        transform.GetChild(4).gameObject.SetActive(false);
    }
    public void Move()
    {
        OpenEyes();
        transform.GetChild(0).localPosition = Vector3.Lerp(transform.GetChild(0).localPosition, new Vector3(0, -1, 0), Time.deltaTime);
        transform.GetChild(1).localPosition = Vector3.Lerp(transform.GetChild(1).localPosition, new Vector3(0, -10, 0), Time.deltaTime);
    }
}
