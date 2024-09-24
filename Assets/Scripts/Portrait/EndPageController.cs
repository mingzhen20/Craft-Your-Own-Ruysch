using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EndPageController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image imageDisplay;  // 显示图片的UI Image
    private RectTransform imageRectTransform;

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

        // 动态添加EventTrigger并关联拖拽事件
        AddEventTrigger(imageDisplay.gameObject, EventTriggerType.BeginDrag, OnBeginDrag);
        AddEventTrigger(imageDisplay.gameObject, EventTriggerType.Drag, OnDrag);
        AddEventTrigger(imageDisplay.gameObject, EventTriggerType.EndDrag, OnEndDrag);
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
}
