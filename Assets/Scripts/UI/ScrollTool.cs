using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// ����ScrollView��ʹ�����ڻ���ʱ����eventtrigger���ڵ�
/// xushi
/// </summary>
public class ScrollTool : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    ScrollRect Scroll;
    //���ڼ�ʱ��Э��
    Coroutine enableShow;


    private void Start()
    {
        Scroll = transform.parent.parent.parent.gameObject.GetComponent<ScrollRect>();
        if (Scroll == null)
            Scroll = transform.parent.parent.parent.parent.gameObject.GetComponent<ScrollRect>();
        if (Scroll == null)
            Debug.Log("Scroll���νű�δ�ҵ�ScrollRect");

        ScrollToolManager.Instance.InitScrollabelBuffItem();
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        Scroll.OnDrag(eventData);
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Scroll.OnBeginDrag(eventData);
        ScrollToolManager.Instance.DisableShow();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Scroll.OnEndDrag(eventData);
        enableShow= StartCoroutine(IE_EnableShow());
    }

    IEnumerator IE_EnableShow()
    {
        float firstTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - firstTime < 0.2f)
        {
            yield return null;
        }
        ScrollToolManager.Instance.EnableShow();
    }

    private void OnDisable()
    {
        if(enableShow!=null)
            StopCoroutine(enableShow);
    }
}