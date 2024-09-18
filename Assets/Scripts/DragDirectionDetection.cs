using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class DragDirectionDetection : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,IPointerDownHandler
{
    public DragDirection dragDirection;
    public FloatWindow floatWindow;


    public void OnPointerDown(PointerEventData eventData)
    {
        floatWindow.SetDragDirection(dragDirection);

    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        floatWindow.OnBeginDrag();
    }

    public void OnDrag(PointerEventData eventData)
    {
      
        floatWindow.OnDrag();

    }

    public void OnEndDrag(PointerEventData eventData)
    {

        floatWindow.OnEndDrag();


    }

   
}
