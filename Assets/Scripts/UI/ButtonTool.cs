using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ���Ƶ�ǰbutton�¼�����
/// xushi
/// </summary>
public class ButtonHighLight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.Find("Image").gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.Find("Image").gameObject.SetActive(false);
    }
}