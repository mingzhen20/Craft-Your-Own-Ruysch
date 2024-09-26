using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EndPageController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image imageDisplay;  // 显示图片的UI Image
    private RectTransform imageRectTransform;
    private float initialDistance;
    private Vector3 initialScale;
    private float minZoom = 0.5f;  // 最小缩放比例
    private float maxZoom = 2.0f;  // 最大缩放比例

    void Start()
    {
        // 加载图片数据
        string imageDataString = PlayerPrefs.GetString("FinalImage", null);
        if (!string.IsNullOrEmpty(imageDataString))
        {
            byte[] imageBytes = System.Convert.FromBase64String(imageDataString);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);

            imageDisplay.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
        else
        {
            Debug.LogError("未找到图片数据！");
        }

        // 获取图片的 RectTransform
        imageRectTransform = imageDisplay.GetComponent<RectTransform>();

        // 确保 Image 对象可以接收 Raycast
        if (!imageDisplay.raycastTarget)
        {
            Debug.LogWarning("Raycast Target 未启用。启用 Raycast Target。");
            imageDisplay.raycastTarget = true;
        }

        // 动态添加拖拽事件
        AddEventTrigger(imageDisplay.gameObject, EventTriggerType.BeginDrag, OnBeginDrag);
        AddEventTrigger(imageDisplay.gameObject, EventTriggerType.Drag, OnDrag);
        AddEventTrigger(imageDisplay.gameObject, EventTriggerType.EndDrag, OnEndDrag);
    }

    void Update()
    {
        HandlePinchZoom(); // 处理缩放手势
    }

    private void AddEventTrigger(GameObject target, EventTriggerType eventType, System.Action<PointerEventData> action)
    {
        EventTrigger trigger = target.GetComponent<EventTrigger>();
        if (trigger == null) trigger = target.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener((eventData) => { action.Invoke((PointerEventData)eventData); });
        trigger.triggers.Add(entry);
    }

    // 开始拖拽时调用
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("开始拖拽");
    }

    // 拖拽过程中调用
    public void OnDrag(PointerEventData eventData)
    {
        if (imageRectTransform != null)
        {
            // 更新图片的位置
            imageRectTransform.anchoredPosition += eventData.delta;
        }
    }

    // 拖拽结束时调用
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("结束拖拽");
    }

    // 处理双指缩放手势
    private void HandlePinchZoom()
    {
        if (Input.touchCount == 2)  // 如果有两个触点
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // 计算两指间的当前距离
            float currentDistance = Vector2.Distance(touchZero.position, touchOne.position);

            // 检查是否是新的缩放手势
            if (touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began)
            {
                initialDistance = currentDistance;
                initialScale = imageRectTransform.localScale;  // 记录初始的缩放比例
            }
            else
            {
                // 计算缩放比例
                if (Mathf.Approximately(initialDistance, 0)) return;  // 避免除以零
                float scaleFactor = currentDistance / initialDistance;

                // 根据缩放比例调整图片的大小
                Vector3 newScale = initialScale * scaleFactor;
                newScale.x = Mathf.Clamp(newScale.x, minZoom, maxZoom);  // 限制缩放比例
                newScale.y = Mathf.Clamp(newScale.y, minZoom, maxZoom);

                imageRectTransform.localScale = newScale;  // 应用缩放
            }
        }
    }
}
