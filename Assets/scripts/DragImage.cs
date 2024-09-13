using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragImage : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public float stepDistance = 10f; // 设置单步间隔长度

    private bool isDragging;
    private RectTransform rectTransform;
    private RectTransform parentRectTransform;
    private Vector2 accumulatedDelta;
    private Vector2 lastPosition;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentRectTransform = transform.parent as RectTransform;
    }


    void Start()
    {

    }


    void Update()
    {
        if (isDragging)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (scroll != 0)
            {
                float rotation = Mathf.Sign(scroll) * 90f;
                parentRectTransform.Rotate(Vector3.forward, rotation);
            }

            if (Input.GetMouseButtonDown(1))
            {
                parentRectTransform.localScale = new Vector3(-1f * parentRectTransform.localScale.x, 1f,1f);
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        parentRectTransform.SetAsLastSibling();
        accumulatedDelta = Vector2.zero;
        lastPosition = eventData.position;

    }

    public void OnDrag(PointerEventData eventData)
    {
        //parentRectTransform.anchoredPosition += eventData.delta;


        Vector2 delta = eventData.position - lastPosition;
        accumulatedDelta += delta;
        //print($"eventData.position:{eventData.position},delta:{delta},accumulatedDelta:{accumulatedDelta}");

        if (Mathf.Abs(accumulatedDelta.x) >= stepDistance)
        {
            float sign = Mathf.Sign(accumulatedDelta.x);//(new Vector2(1, 0) * accumulatedDelta.x).normalized.x;//获取方向,并且需要让取值在1或者-1这两个数
            float moveValue = accumulatedDelta.x - (Mathf.Abs(accumulatedDelta.x) % stepDistance) * sign;
            print("moveValueX:" + moveValue);
            parentRectTransform.anchoredPosition += new Vector2(moveValue, 0);
            accumulatedDelta = new Vector2(accumulatedDelta.x - moveValue, accumulatedDelta.y);

            lastPosition = new Vector2(lastPosition.x + moveValue, lastPosition.y);
        }
        if (Mathf.Abs(accumulatedDelta.y) >= stepDistance)
        {
            float sign = Mathf.Sign(accumulatedDelta.y);
            float moveValue = accumulatedDelta.y - (Mathf.Abs(accumulatedDelta.y) % stepDistance) * sign;
            parentRectTransform.anchoredPosition += new Vector2(0, moveValue);
            accumulatedDelta = new Vector2(accumulatedDelta.x, accumulatedDelta.y - moveValue);

            lastPosition = new Vector2(lastPosition.x, lastPosition.y + moveValue);
            print("moveValueY:" + moveValue);
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        //manager.OnEndDrag(this);
    }

    public void Close(){
        Application.Quit();
    }
}

