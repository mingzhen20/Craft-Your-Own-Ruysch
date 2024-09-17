using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static System.Net.Mime.MediaTypeNames;
using static UnityEngine.RectTransform;



public static class UIUtil
{

	#region  RectTransform相关方法 

	/// <summary>
	/// 设置RectTransform的宽高
	/// </summary>
	/// <param name="rectTransform"></param>
	/// <param name="width"></param>
	/// <param name="height"></param>
	public static void SetRectTransformSizeWithCurrentAnchors(this RectTransform rectTransform, float width, float height)
	{
		if (rectTransform != null)
		{
			rectTransform.SetSizeWithCurrentAnchors(Axis.Horizontal, width);
			rectTransform.SetSizeWithCurrentAnchors(Axis.Vertical, height);
		}
	}

	/// <summary>
	/// 获取屏幕点点在 RectTransform 上的本地坐标
	/// </summary>
	/// <param name="uiCanvas"></param>
	/// <returns></returns>

	public static Vector2 GetScreenPointToLocalPointInRectangle(this RectTransform rectTransform, Vector3 screenPoint, Canvas uiCanvas)
	{
		Vector2 worldPos;
		if (uiCanvas == null)
		{
			return Vector3.one;

		}
		if (uiCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, uiCanvas.worldCamera, out worldPos);
			return worldPos;
		}
		else
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, null, out worldPos);
			return worldPos;
		}
	}


    public static void SetRectTransformPivot(this RectTransform rectTransform, Vector2 pivot)
    {
        Vector3 newPosition = GetPivotWorldPoint(rectTransform, pivot);

        rectTransform.pivot = pivot;

       
        rectTransform.position = newPosition;

    }
    public static Vector3 GetPivotWorldPoint(RectTransform rectTransform, Vector3 newPivot)
    {
        // 获取RectTransform左下角本地坐标  
        Vector3 leftBottomLocalPosition = GetRectTransformLocalCorners(rectTransform)[0];

        // 获取RectTransform的大小  
        Vector2 size = rectTransform.sizeDelta;

     

        // 根据锚点计算出当前宽高
        Vector2 localPivotPoint = new Vector2(newPivot.x * size.x, newPivot.y * size.y);

        // 根据宽高计算本地坐标  
        Vector3 worldPivotPoint = leftBottomLocalPosition + new Vector3(localPivotPoint.x, localPivotPoint.y, 0);

        //将本地坐标转换成世界坐标

        return  rectTransform.TransformPoint(worldPivotPoint);
    }
    /// <summary>
    /// 获取rect四个角的本地坐标坐标 第一个为左下角，第二个为左上角，第三个为右上角，第四个为右下角
    /// </summary>
    /// <param name="rectTransform"></param>
    /// <returns></returns>
    public static Vector3[] GetRectTransformLocalCorners(this RectTransform rectTransform)
    {
        Vector3[] fourCornersArray = new Vector3[4];

        rectTransform.GetLocalCorners(fourCornersArray);

        return fourCornersArray;
    }



   

    /// <summary>
    /// 获取屏幕上的点在 Canvas 上的世界坐标
    /// </summary>
    /// <param name="screenPoint"></param>
    /// <param name="uiCanvas"></param>
    /// <returns></returns>
    public static Vector3 GetScreenPointToWorldPointInRectangle(Vector2 screenPoint, Canvas uiCanvas = null)
    {
        Vector3 worldPos;
        if (uiCanvas == null)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(uiCanvas.GetComponent<RectTransform>(), screenPoint, null, out worldPos);
            return worldPos;

        }
        if (uiCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(uiCanvas.GetComponent<RectTransform>(), screenPoint, uiCanvas.worldCamera, out worldPos);



            return worldPos;
        }
        else
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(uiCanvas.GetComponent<RectTransform>(), screenPoint, null, out worldPos);
            return worldPos;
        }

    }

    #endregion


}

