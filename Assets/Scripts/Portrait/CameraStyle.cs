using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System.IO;
using System;

public class CameraStyle : MonoBehaviour
{
    WebCamTexture webCamTexture;
    public RawImage displayImage;
    public RawImage resultImageDisplay; // 显示风格化图片的UI
    private Texture2D capturedImage;
    private bool hasCaptured = false;
    private string apiKey = "sk-HqrYONEHXZlQ3pbdKQPPXyMAow953gsszzyb7y8W7OzDytuc"; //style transfer API key

    public Button confirmButton;

    void Start()
    {
        StartCamera();
        confirmButton.onClick.AddListener(ConfirmSelection);
    }

    void StartCamera()
    {
        webCamTexture = new WebCamTexture();
        displayImage.texture = webCamTexture;
        webCamTexture.Play();
        hasCaptured = false;
    }

    void StopCamera()
    {
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            webCamTexture.Stop();
        }
    }

    // 捕捉图像
    public void CaptureImage()
    {
        if (!hasCaptured)
        {
            capturedImage = new Texture2D(webCamTexture.width, webCamTexture.height);
            capturedImage.SetPixels(webCamTexture.GetPixels());
            capturedImage.Apply();

            displayImage.texture = capturedImage;

            StopCamera();
            hasCaptured = true;
        }
        else
        {
            StartCamera();
        }
    }

    public void ApplyArtStyle()
    {
        if (capturedImage == null)
        {
            Debug.LogError("没有捕获的图像，无法应用风格。");
            return;
        }

        byte[] imageBytes = capturedImage.EncodeToJPG();
        StartCoroutine(UploadImage(imageBytes));
    }

    IEnumerator UploadImage(byte[] imageBytes)
    {
        // 使用 Style 服务的API URL
        string url = "https://api.stability.ai/v2beta/stable-image/control/style";

        WWWForm form = new WWWForm();
        form.AddBinaryData("image", imageBytes, "image.jpg", "image/jpeg");
        // 设置提示以指导风格迁移过程
        form.AddField("prompt", "Transform this portrait into a detailed Baroque style while maintaining the original facial features and expression.");

        UnityWebRequest request = UnityWebRequest.Post(url, form);
        request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
        request.SetRequestHeader("Accept", "image/*");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D styledTexture = new Texture2D(2, 2);
            styledTexture.LoadImage(request.downloadHandler.data); // 加载API返回的图像数据
            resultImageDisplay.texture = styledTexture;
            Debug.Log("Baroque style portrait created successfully.");

            SaveResultImage(styledTexture); // 图像生成后立即保存
        }
        else
        {
            Debug.LogError("Image upload failed: " + request.error);
        }
    }


    // 保存图像的函数
    void SaveResultImage(Texture2D texture)
    {
        // 使用当前日期和时间生成唯一文件名
        string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff"); // 使用毫秒级时间戳
        string fileName = "resultImage_" + timestamp + ".png";
        string path = Path.Combine(Application.persistentDataPath, fileName); // 使用新的文件名

        byte[] bytes = texture.EncodeToPNG(); // 将Texture2D编码为PNG字节数组
        File.WriteAllBytes(path, bytes); // 写入文件系统
        Debug.Log("Saved Image to: " + path);
    }


    // 点击Confirm后进行背景去除
    void ConfirmSelection()
    {
        if (resultImageDisplay.texture == null)
        {
            Debug.LogError("没有风格化图像，无法进行背景去除。");
            return;
        }

        // 将风格化图像转为Texture2D
        Texture2D texture = resultImageDisplay.texture as Texture2D;
        byte[] imageBytes = texture.EncodeToPNG();
        string base64Image = System.Convert.ToBase64String(imageBytes);
        StartCoroutine(RemoveBackgroundCoroutine(base64Image));
    }

    IEnumerator RemoveBackgroundCoroutine(string base64Image)
    {
        string url = "https://api.remove.bg/v1.0/removebg";
        WWWForm form = new WWWForm();
        form.AddField("size", "auto");

        // 从 Base64 解码成字节数组
        byte[] imageBytes = System.Convert.FromBase64String(base64Image);

        // 使用 image_file 传递图片数据，而不是 image_file_b64
        form.AddBinaryData("image_file", imageBytes, "image.png", "image/png");

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        www.SetRequestHeader("X-Api-Key", "iZyP9GV3kcRfuTmeUwpXysV6");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            byte[] resultImageBytes = www.downloadHandler.data;
            PlayerPrefs.SetString("FinalImage", System.Convert.ToBase64String(resultImageBytes));

            // 跳转到 EndPage 场景
            UnityEngine.SceneManagement.SceneManager.LoadScene("EndPage");
        }
        else
        {
            Debug.LogError("背景去除失败: " + www.error);
            Debug.LogError("服务器响应: " + www.downloadHandler.text);
        }
    }
}