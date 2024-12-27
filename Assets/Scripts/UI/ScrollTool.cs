using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 控制ScrollView，使其能在滑动时无视eventtrigger的遮挡
/// xushi
/// </summary>
public class ScrollTool : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    ScrollRect Scroll;
    //用于计时的协程
    Coroutine enableShow;


    private void Start()
    {
        Scroll = transform.parent.parent.parent.gameObject.GetComponent<ScrollRect>();
        if (Scroll == null)
            Scroll = transform.parent.parent.parent.parent.gameObject.GetComponent<ScrollRect>();
        if (Scroll == null)
            Debug.Log("Scroll屏蔽脚本未找到ScrollRect");

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