using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtworkManager : MonoBehaviour
{
    public GameObject enlargedImageDisplay;   // 用于放大显示的画作区域
    public RectTransform scrollViewContent;   // ScrollView 中的内容，包含所有的画作
    public Transform flowerDisplayBar;        // 用于展示花朵的区域
    public GameObject flowerButtonPrefab;     // 花朵按钮的预制体
    public List<Button> allFlowerAreaButtons; // 所有 flowerArea 按钮的列表
    private Image enlargedImageComponent;     // 用于显示放大的画作或花朵

    void Start()
    {
        // 获取 EnlargedArtworkDisplay 的 Image 组件
        enlargedImageComponent = enlargedImageDisplay.GetComponent<Image>();

        // 遍历 ScrollView 中的每个画作按钮，并添加点击事件
        foreach (Transform imageTransform in scrollViewContent)
        {
            Button imageButton = imageTransform.GetComponent<Button>();
            Artwork artwork = imageTransform.GetComponent<Artwork>();  // 获取每个 Artwork 脚本
            if (imageButton != null && artwork != null)
            {
                imageButton.onClick.AddListener(() => OnArtworkClick(imageButton, artwork));
            }
            else
            {
                Debug.LogWarning("Button 或 Artwork 组件未找到在: " + imageTransform.name);
            }
        }

    }

    // 当点击某个画作时调用此方法
    void OnArtworkClick(Button clickedButton, Artwork artwork)
    {
        Image clickedImage = clickedButton.GetComponent<Image>();

        if (clickedImage != null)
        {
            // 显示选中的画作
            enlargedImageComponent.sprite = clickedImage.sprite;

            float originalWidth = clickedImage.sprite.texture.width;
            float originalHeight = clickedImage.sprite.texture.height;
            float aspectRatio = originalWidth / originalHeight;

            float displayHeight = 500f;
            float displayWidth = displayHeight * aspectRatio;

            enlargedImageComponent.rectTransform.sizeDelta = new Vector2(displayWidth, displayHeight);
            enlargedImageComponent.rectTransform.anchoredPosition = Vector2.zero;

            enlargedImageDisplay.SetActive(true);


            // 为当前画作的每个花朵区域绑定点击事件并激活按钮
            foreach (RectTransform flowerArea in artwork.flowerAreas)
            {
                Button flowerButton = flowerArea.GetComponent<Button>();
                if (flowerButton == null)
                {
                    flowerButton = flowerArea.gameObject.AddComponent<Button>(); // 动态添加 Button 组件
                }

                // 移除之前的事件，防止重复绑定
                flowerButton.onClick.RemoveAllListeners();

                // 激活该花朵区域按钮
                flowerButton.gameObject.SetActive(true);

                // 使用 Lambda 表达式延迟传递参数，确保点击时才调用事件
                int index = System.Array.IndexOf(artwork.flowerAreas, flowerArea);  // 找到花朵索引
                flowerButton.onClick.AddListener(() => OnFlowerAreaClick(artwork.flowerSprites[index]));
            }
        }
        else
        {
            Debug.LogError("在 " + clickedButton.name + " 上未找到 Image 组件");
        }
    }

    // 点击花朵区域后调用此方法
    void OnFlowerAreaClick(Sprite flowerSprite)
    {
    Debug.Log("点击了花朵区域，花朵：" + flowerSprite.name + " 被添加到展示栏");

    // 创建 flowerButton 并将其添加到 DisplayBar 中
    GameObject flowerButton = Instantiate(flowerButtonPrefab, flowerDisplayBar);

    // 设置按钮的图片
    flowerButton.GetComponent<Image>().sprite = flowerSprite;

    // 调整按钮的 RectTransform，确保它在 DisplayBar 的正确位置
    RectTransform rectTransform = flowerButton.GetComponent<RectTransform>();
    rectTransform.localScale = Vector3.one; // 确保按钮的缩放比例为正常
    rectTransform.anchoredPosition = Vector2.zero; // 确保按钮在 DisplayBar 中的位置

    // 调试日志，确认花朵按钮已添加到 DisplayBar
    Debug.Log("花朵已被添加到 DisplayBar：" + flowerSprite.name);
}
}