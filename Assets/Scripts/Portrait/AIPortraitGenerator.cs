using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System; 

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
    private string apiKey = "sk-EXcxgGSXe57Id8HMNTvxAOQQ3WEqVWgUCdS0BfINZhmhI90j";  // 替换为你的实际API密钥

    public Sprite placeholderSprite1;  // 第一张占位图片
    public Sprite placeholderSprite2;  // 第二张占位图片
    private GameObject selectedImage = null;  // 存储用户选中的图片对象
    public string removeBgApiKey = "55TkVa4x5A8BAd2PFMkMi8Am";  // 你的remove.bg API密钥
    public string endPageSceneName = "EndPage";  // 结束页面的场景名称

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

        // 生成唯一文件名
        string uniqueFileName = "generated_image_" + Guid.NewGuid().ToString() + ".png";
        string filePath = Path.Combine(Application.persistentDataPath, uniqueFileName);

        StartCoroutine(SendGenerateRequest(finalPrompt, filePath));
        // StartCoroutine(SendGenerateRequest(finalPrompt));

    }

    IEnumerator SendGenerateRequest(string prompt, string filePath)
    {
        loadingIcon.SetActive(true);  // 显示Loading动画

        string boundary = "------------------------" + System.DateTime.Now.Ticks.ToString("x");
        byte[] formData = GetMultipartFormData(boundary, prompt, "png", "sd3-medium");

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(formData);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Authorization", "Bearer " + apiKey);
        request.SetRequestHeader("Content-Type", "multipart/form-data; boundary=" + boundary);
        request.SetRequestHeader("Accept", "image/*");

        yield return request.SendWebRequest();

        loadingIcon.SetActive(false);  // 隐藏Loading动画

        if (request.result == UnityWebRequest.Result.Success)
        {
            byte[] imageBytes = request.downloadHandler.data;

            // 创建可读的 Texture2D
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            texture.LoadImage(imageBytes);

            // 确保 Texture2D 是可读的
            if (texture.isReadable)
            {
                Debug.Log("Texture2D is readable");
            }
            else
            {
                Debug.LogError("Texture2D is not readable");
            }

            // 创建动态的图片 GameObject
            GameObject newImage = Instantiate(imagePrefab, container.transform);
            newImage.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            // 添加到 generatedImages 列表中
            generatedImages.Add(newImage);

            // 确保 Image 的 Raycast Target 被启用
            Image imgComponent = newImage.GetComponent<Image>();
            if (imgComponent != null)
            {
                imgComponent.raycastTarget = true;  // 确保 Raycast Target 被启用
            }

            // 添加点击事件，通过点击选择图片
            Button imageButton = newImage.GetComponent<Button>();
            if (imageButton != null)
            {
                imageButton.onClick.AddListener(() => OnImageClicked(newImage));
            }
            else
            {
                Debug.LogError("Button component is missing on the generated image prefab.");
            }

            // 保存图片到指定文件路径
            File.WriteAllBytes(filePath, imageBytes);
            Debug.Log("图像已保存到: " + filePath);

            generationCount++;

            // 禁用生成按钮
            if (generationCount >= maxGenerations)
            {
                generateButton.interactable = false;  // 禁用生成按钮
                Debug.Log("您已经生成了两张图片，请在其中选择一张。");
            }
        }
        else
        {
            Debug.LogError("请求失败: " + request.error);
        }
    }



    // IEnumerator SendGenerateRequest(string prompt)
    // {
    //     loadingIcon.SetActive(true);  // 显示Loading动画

    //     // 模拟网络请求之前的延迟
    //     yield return new WaitForSeconds(2);  // 假设等待2秒作为生成时间

    //     loadingIcon.SetActive(false);  // 隐藏Loading动画

    //     // 动态生成占位图片
    //     GameObject newImage = Instantiate(imagePrefab, container.transform);
    //     RectTransform rect = newImage.GetComponent<RectTransform>();

    //     // 设置图片的大小
    //     rect.sizeDelta = new Vector2(256, 256);  // 设置为合适的大小

    //     // 设置图片的锚点和居中
    //     rect.anchorMin = new Vector2(0.5f, 0.5f);
    //     rect.anchorMax = new Vector2(0.5f, 0.5f);
    //     rect.pivot = new Vector2(0.5f, 0.5f);
    //     rect.anchoredPosition = Vector2.zero;

    //     // 随机选择占位图片
    //     Sprite selectedSprite = (UnityEngine.Random.Range(0, 2) == 0) ? placeholderSprite1 : placeholderSprite2;

    //     // 设置占位图片
    //     Image imageComponent = newImage.GetComponent<Image>();
    //     if (imageComponent != null)
    //     {
    //         imageComponent.sprite = selectedSprite;  // 设置随机选中的占位图片
    //     }
    //     else
    //     {
    //         Debug.LogError("找不到Image组件，请确保ImagePrefab具有Image组件。");
    //     }

    //     // 添加点击事件，通过点击选择图片
    //     Button imageButton = newImage.GetComponent<Button>();
    //     if (imageButton != null)
    //     {
    //         imageButton.onClick.AddListener(() => OnImageClicked(newImage));
    //     }

    //     generatedImages.Add(newImage);
    //     generationCount++;

    //     // 禁用生成按钮
    //     if (generationCount >= maxGenerations)
    //     {
    //         generateButton.interactable = false;  // 禁用生成按钮
    //         Debug.Log("您已经生成了两张图片，请在其中选择一张。");
    //     }
    // }



    void OnImageClicked(GameObject clickedImage)
    {
        // 重置所有图片状态
        foreach (GameObject image in generatedImages)
        {
            Image img = image.GetComponent<Image>();
            if (img != null)
            {
                img.color = Color.white;  // 将所有图片恢复默认颜色
            }
        }

        // 高亮显示被选中的图片
        Image clickedImgComponent = clickedImage.GetComponent<Image>();
        if (clickedImgComponent != null)
        {
            clickedImgComponent.color = Color.Lerp(clickedImgComponent.color, Color.black, 0.3f);  // 加深颜色
        }

        // 记录选中的图片
        selectedImage = clickedImage;

        Debug.Log("选中的图片：" + clickedImage.name);
    }

    void ConfirmSelection()
    {
        if (selectedImage != null)
        {
            Debug.Log("最终选择的图片是：" + selectedImage.name);

            // 获取用户选中的图片的 Sprite
            Image img = selectedImage.GetComponent<Image>();
            if (img != null && img.sprite != null)
            {
                // 获取图片的 Texture2D
                Texture2D originalTexture = img.sprite.texture;

                // 创建一个可读的 Texture2D 副本
                Texture2D readableTexture = new Texture2D(originalTexture.width, originalTexture.height, originalTexture.format, false);
                Graphics.CopyTexture(originalTexture, readableTexture);

                // 将 Texture2D 转换为 byte[]
                byte[] imageBytes = readableTexture.EncodeToPNG();

                // 保存图片为文件
                string filePath = Path.Combine(Application.persistentDataPath, "selected_image.png");
                File.WriteAllBytes(filePath, imageBytes);
                Debug.Log("选中的图片路径为：" + filePath);

                // 开始背景去除处理，并传入选中图片的路径
                StartCoroutine(RemoveBackgroundCoroutine(filePath));
            }
            else
            {
                Debug.LogError("无法获取选中的图片！");
            }
        }
        else
        {
            Debug.Log("未选择任何图片！");
        }
    }

    IEnumerator RemoveBackgroundCoroutine(string imagePath)
    {
        Debug.Log("开始背景去除，路径：" + imagePath);

        // 创建API请求
        WWWForm form = new WWWForm();
        form.AddField("size", "auto");

        // 检查文件是否存在
        if (!System.IO.File.Exists(imagePath))
        {
            Debug.LogError("无法找到图片文件：" + imagePath);
            yield break;
        }

        // 读取图片文件并添加到请求
        byte[] imageData = System.IO.File.ReadAllBytes(imagePath);
        form.AddBinaryData("image_file", imageData, "image.png", "image/png");

        // 创建并发送请求
        UnityWebRequest www = UnityWebRequest.Post("https://api.remove.bg/v1.0/removebg", form);
        www.SetRequestHeader("X-Api-Key", removeBgApiKey);

        // 显示loading图标或动画
        loadingIcon.SetActive(true);

        yield return www.SendWebRequest();

        // 隐藏loading图标
        loadingIcon.SetActive(false);

        if (www.result == UnityWebRequest.Result.Success)
        {
            byte[] imageBytes = www.downloadHandler.data;
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);

            // 将处理后的图片设置为 Sprite
            Sprite finalImageSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            selectedImage.GetComponent<Image>().sprite = finalImageSprite;

            // 保存图片数据到本地文件
            string filePath = Path.Combine(Application.persistentDataPath, "processed_image.png");
            File.WriteAllBytes(filePath, imageBytes);
            Debug.Log("图片已保存到: " + filePath);

            // 保存图片数据到 PlayerPrefs
            PlayerPrefs.SetString("FinalImage", System.Convert.ToBase64String(imageBytes));

            // 跳转到 EndPage 场景
            UnityEngine.SceneManagement.SceneManager.LoadScene(endPageSceneName);
            Debug.Log("跳转到 EndPage 场景");
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
