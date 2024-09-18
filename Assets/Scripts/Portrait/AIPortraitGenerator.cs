using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class StableDiffusionGenerator : MonoBehaviour
{
    public TMP_InputField promptInput;  // TextMeshPro的输入框
    public Image portraitPreview;  // 用于显示生成的图像
    public Button generateButton;  // 生成按钮

    private string apiUrl = "https://api.stability.ai/v2beta/stable-image/generate/sd3";  // 使用SD3模型的API
    private string apiKey = "sk-Zfws3GNX3jLVG1aNXlPQWpeCurY1yCjrQgviWLgSu0MJI1pN";  // 替换为你的实际API密钥

    void Start()
    {
        generateButton.onClick.AddListener(GenerateImage);
    }

    void GenerateImage()
    {
        // Rachel Ruysch 风格的巴洛克人物肖像，不包含背景或其他物品
        string defaultPrompt = "A self-portrait in the style of Rachel Ruysch, baroque style, soft and natural lighting, rich colors, intricate details, smooth textures, 17th century oil painting, head and shoulders portrait, no background, focus on the face and expression";

        // 获取用户输入的额外关键词
        string userPrompt = promptInput.text;
        string finalPrompt = defaultPrompt + ", " + userPrompt;

        StartCoroutine(SendGenerateRequest(finalPrompt));
    }


    IEnumerator SendGenerateRequest(string prompt)
    {
        // 手动构建 multipart/form-data 请求体
        string boundary = "------------------------" + System.DateTime.Now.Ticks.ToString("x");
        byte[] formData = GetMultipartFormData(boundary, prompt, "png", "sd3-medium");

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(formData);
        request.downloadHandler = new DownloadHandlerBuffer();

        // 设置请求头
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);
        request.SetRequestHeader("Content-Type", "multipart/form-data; boundary=" + boundary);
        request.SetRequestHeader("Accept", "image/*");  // 请求返回图像

        // 发送请求并等待响应
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // 获取返回的图像数据
            byte[] imageBytes = request.downloadHandler.data;

            // 保存图片到本地
            string path = Path.Combine(Application.persistentDataPath, "generated_image.png");
            File.WriteAllBytes(path, imageBytes);
            Debug.Log("图像已保存到: " + path);

            // 在Unity中显示生成的图像
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);
            portraitPreview.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
        else
        {
            Debug.LogError("请求失败: " + request.error);
            Debug.LogError("响应内容: " + request.downloadHandler.text);
        }
    }

    // 手动构建 multipart/form-data 数据
    private byte[] GetMultipartFormData(string boundary, string prompt, string outputFormat, string model)
    {
        // 构建 multipart 数据
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

        // 将数据转换为字节数组
        return Encoding.UTF8.GetBytes(bodyBuilder.ToString());
    }
}
