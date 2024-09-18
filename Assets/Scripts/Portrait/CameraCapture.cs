using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class CameraCapture : MonoBehaviour
{
    WebCamTexture webCamTexture;
    public RawImage displayImage;
    public RawImage resultImageDisplay; // 显示风格化图片的UI
    private Texture2D capturedImage;
    private bool hasCaptured = false;
    private string apiKey = "R55LYUGpVsaLPIXUwcrhJ4wrG2ZX5KdE46jzs3m4"; // 在这里定义API Key


    void Start()
    {
        StartCamera();
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

    // 获取可用的风格列表
    public void GetAvailableStyles()
    {
        StartCoroutine(GetStyles());
    }

    // 调用DeepArt API获取风格列表
    IEnumerator GetStyles()
    {
        UnityWebRequest request = UnityWebRequest.Get("https://api.deeparteffects.com/v1/noauth/styles");
        request.SetRequestHeader("x-api-key", apiKey);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("风格列表获取成功：" + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("获取风格列表失败: " + request.error);
        }
    }

    // 应用艺术风格
    public void ApplyArtStyle()
    {
        if (capturedImage == null)
        {
            Debug.LogError("没有捕获的图像，无法应用风格。");
            return;
        }

        // 将捕获的图像转换为Base64
        byte[] imageBytes = capturedImage.EncodeToJPG();
        string base64Image = System.Convert.ToBase64String(imageBytes);

        // 调用API应用风格（示例使用巴洛克风格ID）
        StartCoroutine(UploadImage(base64Image, "c7984b32-1560-11e7-afe2-06d95fe194ed"));  // 替换为你获取到的风格ID
    }

    // 上传图片并应用风格
    IEnumerator UploadImage(string base64Image, string styleId)
    {
        string url = "https://api.deeparteffects.com/v1/noauth/upload";

        // 创建请求体
        string jsonRequestBody = JsonUtility.ToJson(new
        {
            styleId = styleId,
            imageBase64Encoded = base64Image,
            imageSize = "512"
        });

        byte[] postData = Encoding.UTF8.GetBytes(jsonRequestBody);
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(postData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("x-api-key", apiKey);

        Debug.Log("上传的请求体: " + jsonRequestBody);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("图像上传成功");
            // 解析返回的 submissionId
            var response = JsonUtility.FromJson<DeepArtResponse>(request.downloadHandler.text);
            StartCoroutine(GetResult(response.submissionId));
        }
        else
        {
            Debug.LogError("图像上传失败: " + request.error);
            Debug.LogError("服务器响应: " + request.downloadHandler.text);
        }
    }

    // 获取风格化后的图像结果
    IEnumerator GetResult(string submissionId)
    {
        string url = $"https://api.deeparteffects.com/v1/noauth/result?submissionId={submissionId}";

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var response = JsonUtility.FromJson<DeepArtResult>(request.downloadHandler.text);

            if (response.status == "finished")
            {
                StartCoroutine(DownloadImage(response.url));
            }
            else
            {
                Debug.LogError("风格转换仍在处理中。状态: " + response.status);
            }
        }
        else
        {
            Debug.LogError("获取风格化结果失败: " + request.error);
            Debug.LogError("服务器响应: " + request.downloadHandler.text);
        }
    }

    // 下载并显示风格化后的图像
    IEnumerator DownloadImage(string imageUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D styledTexture = DownloadHandlerTexture.GetContent(request);
            resultImageDisplay.texture = styledTexture;
            Debug.Log("风格化图片下载并显示成功。");
        }
        else
        {
            Debug.LogError("下载风格化图像失败: " + request.error);
        }
    }
}

// API响应的帮助类
[System.Serializable]
public class DeepArtResponse
{
    public string submissionId;
}

[System.Serializable]
public class DeepArtResult
{
    public string status;
    public string url;
}
