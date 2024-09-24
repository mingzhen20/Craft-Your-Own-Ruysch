using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EndPageController : MonoBehaviour, IDragHandler
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
    }

    // 拖拽图片
    public void OnDrag(PointerEventData eventData)
    {
        if (imageRectTransform != null)
        {
            // 更新图片的位置
            imageRectTransform.anchoredPosition += eventData.delta;
        }
    }
}
