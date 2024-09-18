using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotationDetection : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public FloatWindow floatWindow;
    private  RectTransform rectTransform;
    private float offsetAngle;

    private void Awake()
    {
        rectTransform= floatWindow.GetComponent<RectTransform>();
       
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        floatWindow.SetPovit(Vector2.one*0.5f);

        Vector3 worldPosition;
        if (floatWindow.UICanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, null, out worldPosition);
        }
        else
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, floatWindow.UICanvas.worldCamera, out worldPosition);

        }



        worldPosition.z = rectTransform.position.z;
        Vector3 dir = worldPosition - rectTransform.position;

 

        offsetAngle = Vector3.Angle(dir, rectTransform.up);
       
    }


    // ������϶�ʱ  
    public void OnDrag(PointerEventData eventData)
    {
        Vector3 worldPosition;
        if (floatWindow.UICanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, null, out worldPosition);
        }
        else { 
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, floatWindow.UICanvas.worldCamera, out  worldPosition);

        }



        worldPosition.z = rectTransform.position.z;
        Vector3 dir = worldPosition - rectTransform.position;

       
         rectTransform.rotation = Quaternion.LookRotation(rectTransform.forward, dir)*Quaternion.Euler( Vector3.forward* offsetAngle);
    }

   
    public void OnEndDrag(PointerEventData eventData)
    {
       
    }

 

  
}
