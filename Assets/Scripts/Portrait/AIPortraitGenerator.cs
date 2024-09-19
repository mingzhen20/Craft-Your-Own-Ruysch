using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class StableDiffusionGenerator : MonoBehaviour
{
    public TMP_InputField promptInput;  // TextMeshPro的输入框
    public GameObject container;  // 用于存放图片的容器，带有HorizontalLayoutGroup
    public Button generateButton;  // 生成按钮
    public Button confirmButton;   // 确认选择按钮
    public GameObject imagePrefab;  // Image的预制体
    public GameObject loadingIcon;  // Loading图标或动画

    private int generationCount = 0;  // 当前生成计数
    private const int maxGenerations = 2;  // 最多生成两次
    private List<GameObject> generatedImages = new List<GameObject>();  // 保存生成的图片对象



    private string apiUrl = "https://api.stability.ai/v2beta/stable-image/generate/sd3";  // 使用SD3模型的API
    private string apiKey = "sk-Zfws3GNX3jLVG1aNXlPQWpeCurY1yCjrQgviWLgSu0MJI1pN";  // 替换为你的实际API密钥

    public Sprite placeholderSprite;  // 占位图片，拖动进Inspector面板

    void Start()
    {
        generateButton.onClick.AddListener(GenerateImage);
        confirmButton.onClick.AddListener(ConfirmSelection);
        loadingIcon.SetActive(false); // 确保在开始时它是隐藏的
    }

    void GenerateImage()
    {
        if (generationCount >= maxGenerations)
        {
            Debug.Log("您已经生成了两张图片，无法再生成。请选择其中一张。");
            return;
        }

        string defaultPrompt = "A self-portrait in the style of Rachel Ruysch, baroque style, soft and natural lighting, rich colors, intricate details, smooth textures, 17th century oil painting, head and shoulders portrait, no background, focus on the face and expression";
        string userPrompt = promptInput.text;
        string finalPrompt = defaultPrompt + ", " + userPrompt;

        StartCoroutine(SendGenerateRequest(finalPrompt));
    }

    // IEnumerator SendGenerateRequest(string prompt)
    // {

    //     loadingIcon.SetActive(true);  // 显示Loading动画

    //     string boundary = "------------------------" + System.DateTime.Now.Ticks.ToString("x");
    //     byte[] formData = GetMultipartFormData(boundary, prompt, "png", "sd3-medium");

    //     UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
    //     request.uploadHandler = new UploadHandlerRaw(formData);
    //     request.downloadHandler = new DownloadHandlerBuffer();

    //     request.SetRequestHeader("Authorization", "Bearer " + apiKey);
    //     request.SetRequestHeader("Content-Type", "multipart/form-data; boundary=" + boundary);
    //     request.SetRequestHeader("Accept", "image/*");

    //     yield return request.SendWebRequest();

    //     loadingIcon.SetActive(false);  // 隐藏Loading动画

    //     if (request.result == UnityWebRequest.Result.Success)
    //     {
    //         byte[] imageBytes = request.downloadHandler.data;
    //         Texture2D texture = new Texture2D(2, 2);
    //         texture.LoadImage(imageBytes);
    //         generatedImages.Add(texture);

    //         // 创建动态的图片和Toggle
    //         GameObject newImage = Instantiate(imagePrefab, container.transform);
    //         newImage.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

    //         GameObject newToggle = Instantiate(togglePrefab, container.transform);
    //         toggles.Add(newToggle.GetComponent<Toggle>());

    //         string path = Path.Combine(Application.persistentDataPath, "generated_image.png");
    //         File.WriteAllBytes(path, imageBytes);
    //         Debug.Log("图像已保存到: " + path);

    //         generationCount++;

    //         // 禁用生成按钮
    //         if (generationCount >= maxGenerations)
    //         {
    //             generateButton.interactable = false;  // 禁用生成按钮
    //             Debug.Log("您已经生成了两张图片，请在其中选择一张。");
    //         }
    //     }
    //     else
    //     {
    //         Debug.LogError("请求失败: " + request.error);
    //         Debug.LogError("响应内容: " + request.downloadHandler.text);
    //     }
    // }

    IEnumerator SendGenerateRequest(string prompt)
    {
        loadingIcon.SetActive(true);  // 显示Loading动画

        // 模拟网络请求之前的延迟
        yield return new WaitForSeconds(2);  // 假设等待2秒作为生成时间

        loadingIcon.SetActive(false);  // 隐藏Loading动画

        // 动态生成占位图片
        GameObject newImage = Instantiate(imagePrefab, container.transform);
        RectTransform rect = newImage.GetComponent<RectTransform>();

        // 设置图片的大小
        rect.sizeDelta = new Vector2(256, 256);  // 设置为合适的大小

        // 设置图片的锚点和居中
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;

        // 设置占位图片
        Image imageComponent = newImage.GetComponent<Image>();
        if (imageComponent != null)
        {
            imageComponent.sprite = placeholderSprite;  // 设置占位图片
        }
        else
        {
            Debug.LogError("找不到Image组件，请确保ImagePrefab具有Image组件。");
        }

        // 添加点击事件，通过点击选择图片
        Button imageButton = newImage.GetComponent<Button>();
        if (imageButton != null)
        {
            imageButton.onClick.AddListener(() => OnImageClicked(newImage));
        }

        generatedImages.Add(newImage);
        generationCount++;

        // 禁用生成按钮
        if (generationCount >= maxGenerations)
        {
            generateButton.interactable = false;  // 禁用生成按钮
            Debug.Log("您已经生成了两张图片，请在其中选择一张。");
        }
    }


    void OnImageClicked(GameObject clickedImage)
    {
        // 遍历所有图片，重置其他图片的状态
        foreach (GameObject image in generatedImages)
        {
            Image img = image.GetComponent<Image>();
            if (img != null)
            {
                img.color = Color.white;  // 未选中的图片颜色恢复
            }
        }

        // 将被点击的图片颜色加深
        Image clickedImgComponent = clickedImage.GetComponent<Image>();
        if (clickedImgComponent != null)
        {
            // 保持原始颜色并加深
            Color originalColor = clickedImgComponent.color;
            Color darkerColor = Color.Lerp(originalColor, Color.black, 0.3f);  // 0.3f 表示加深的程度
            clickedImgComponent.color = darkerColor;
        }

        Debug.Log("选中的图片：" + clickedImage.name);
    }


    void ConfirmSelection()
    {
        foreach (GameObject image in generatedImages)
        {
            Image img = image.GetComponent<Image>();
            if (img != null && img.color == Color.green)
            {
                Debug.Log("最终选择的图片是：" + image.name);
                break;
            }
        }
    }

    private byte[] GetMultipartFormData(string boundary, string prompt, string outputFormat, string model)
    {
        StringBuilder bodyBuilder = new StringBuilder();
        bodyBuilder.AppendLine("--" + boundary);
        bodyBuilder.AppendLine("Content-Disposition: form-data; name=\"prompt\"");
        bodyBuilder.AppendLine();
        bodyBuilder.AppendLine(prompt);

        bodyBuilder.AppendLine("--" + boundary);
        bodyBuilder.AppendLine("Content-Disposition: form-data; name=\"output_format\"");
        bodyBuilder.AppendLine();
        bodyBuilder.AppendLine(outputFormat);

        bodyBuilder.AppendLine("--" + boundary);
        bodyBuilder.AppendLine("Content-Disposition: form-data; name=\"model\"");
        bodyBuilder.AppendLine();
        bodyBuilder.AppendLine(model);

        bodyBuilder.AppendLine("--" + boundary + "--");

        return Encoding.UTF8.GetBytes(bodyBuilder.ToString());
    }
}
