using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum DragDirection
{
    None,
    Left,
    Top,
    Right,
    Bottom,
    LeftTop,
    LeftBottom,
    RightTop,
    RightBottom,
    Center,
}

public class FloatWindow : MonoBehaviour
{
    public Vector2 minSize = new Vector2(540, 280);

    private RectTransform rectTransform;
    private DragDirection dragDirection = DragDirection.None;
    private Vector2 lastPos;
    private Vector3 offset;

    private Canvas canvas;
    public Canvas UICanvas
    {
        get
        {
            return canvas;
        }
    }

    // 添加对3D模型的引用
    public Transform modelTransform;

    // 保存模型的初始缩放比例
    private Vector3 initialModelScale;

    void Start()
    {
        canvas = FindObjectOfType<Canvas>();
        rectTransform = GetComponent<RectTransform>();

        // 如果没有在Inspector中指定modelTransform，可以在这里查找
        if (modelTransform == null)
        {
            // 假设模型是当前对象的子对象，名称为"Model"
            modelTransform = transform.Find("Model");
        }

        if (modelTransform != null)
        {
            // 记录模型的初始缩放比例
            initialModelScale = modelTransform.localScale;
        }
    }

    private void FloatingWindow(DragDirection dragDirection, Vector2 offsetValue)
    {
        Vector2 size = Vector2.zero;
        switch (dragDirection)
        {
            case DragDirection.None:
                return;
            case DragDirection.Left:
                offsetValue.y = 0;
                size = rectTransform.rect.size - new Vector2(offsetValue.x, offsetValue.y);
                break;
            case DragDirection.Top:
                offsetValue.x = 0;
                size = rectTransform.rect.size + new Vector2(offsetValue.x, offsetValue.y);
                break;
            case DragDirection.Right:
                offsetValue.y = 0;
                size = rectTransform.rect.size + new Vector2(offsetValue.x, offsetValue.y);
                break;
            case DragDirection.Bottom:
                offsetValue.x = 0;
                size = rectTransform.rect.size - new Vector2(offsetValue.x, offsetValue.y);
                break;
            case DragDirection.LeftTop:
                size = rectTransform.rect.size + new Vector2(-offsetValue.x, offsetValue.y);
                break;
            case DragDirection.LeftBottom:
                size = rectTransform.rect.size - new Vector2(offsetValue.x, offsetValue.y);
                break;
            case DragDirection.RightTop:
                size = rectTransform.rect.size + new Vector2(offsetValue.x, offsetValue.y);
                break;
            case DragDirection.RightBottom:
                size = rectTransform.rect.size + new Vector2(offsetValue.x, -offsetValue.y);
                break;
            default:
                break;
        }
        size.x = Mathf.Max(size.x, minSize.x);
        size.y = Mathf.Max(size.y, minSize.y);

        // 调整RectTransform的大小
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);

        // 调整模型的缩放和位置
        AdjustModelScaleAndPosition();
    }

    public void SetDragDirection(DragDirection dragDirection)
    {
        this.dragDirection = dragDirection;

        switch (dragDirection)
        {
            case DragDirection.None:
                return;
            case DragDirection.Left:
                SetPovit(new Vector2(1, 0.5f));
                break;
            case DragDirection.Top:
                SetPovit(new Vector2(0.5f, 0));
                break;
            case DragDirection.Right:
                SetPovit(new Vector2(0, 0.5f));
                break;
            case DragDirection.Bottom:
                SetPovit(new Vector2(0.5f, 1f));
                break;
            case DragDirection.LeftTop:
                SetPovit(new Vector2(1f, 0f));
                break;
            case DragDirection.LeftBottom:
                SetPovit(new Vector2(1f, 1f));
                break;
            case DragDirection.RightTop:
                SetPovit(new Vector2(0f, 0f));
                break;
            case DragDirection.RightBottom:
                SetPovit(new Vector2(0f, 1f));
                break;
            case DragDirection.Center:
                SetPovit(new Vector2(0.5f, 0.5f));
                break;
            default:
                break;
        }
    }

    public void SetPovit(Vector2 pivot)
    {
        if (rectTransform == null) return;

        Vector2 size = rectTransform.rect.size;
        Vector2 deltaPivot = rectTransform.pivot - pivot;
        Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
        rectTransform.pivot = pivot;
        rectTransform.localPosition -= deltaPosition;
    }

    public void OnBeginDrag()
    {
        transform.SetAsLastSibling();

        if (dragDirection == DragDirection.Center)
        {
            offset = UIUtil.GetScreenPointToWorldPointInRectangle(Input.mousePosition, UICanvas);
            offset = new Vector3(transform.position.x, transform.position.y, offset.z) - offset;
        }
        else
        {
            lastPos = rectTransform.GetScreenPointToLocalPointInRectangle(Input.mousePosition, UICanvas);
        }
    }

    public void OnDrag()
    {
        if (dragDirection == DragDirection.Center)
        {
            transform.position = UIUtil.GetScreenPointToWorldPointInRectangle(Input.mousePosition, UICanvas) + offset;
        }
        else
        {
            Vector3 mousePosition = Input.mousePosition;

            Vector2 offset = rectTransform.GetScreenPointToLocalPointInRectangle(mousePosition, UICanvas) - lastPos;
            lastPos = rectTransform.GetScreenPointToLocalPointInRectangle(mousePosition, UICanvas);
            FloatingWindow(dragDirection, offset);
        }
    }

    public void OnEndDrag()
    {
        dragDirection = DragDirection.None;
    }

    // 新增方法：调整模型的缩放和位置
    private void AdjustModelScaleAndPosition()
    {
        if (modelTransform == null)
            return;

        // 获取RectTransform的尺寸（以世界单位计算）
        Vector2 rectSize = rectTransform.rect.size;
        Vector2 worldSize = new Vector2(rectSize.x * rectTransform.lossyScale.x, rectSize.y * rectTransform.lossyScale.y);

        // 计算模型的新缩放比例
        // 假设模型的初始尺寸适配于最小尺寸的RectTransform
        float scaleFactorX = worldSize.x / minSize.x;
        float scaleFactorY = worldSize.y / minSize.y;

        // 为了保持比例，取缩放因子的最小值
        float scaleFactor = Mathf.Min(scaleFactorX, scaleFactorY);

        modelTransform.localScale = initialModelScale * scaleFactor;

        // 将模型的位置设置为RectTransform的中心位置
        Vector3 worldPosition = rectTransform.position;
        modelTransform.position = worldPosition;
    }
}
