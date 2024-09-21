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
    public string styleId = "";
    private Texture2D capturedImage;
    private bool hasCaptured = false;
    private string apiKey = "R55LYUGpVsaLPIXUwcrhJ4wrG2ZX5KdE46jzs3m4";

    public Button getStylesButton;

    void Start()
    {
        StartCamera();

        getStylesButton.onClick.AddListener(OnGetStylesButtonClicked);
    }

    // 点击按钮时调用的函数
    void OnGetStylesButtonClicked()
    {
        StartCoroutine(GetStyles()); // 开始获取样式
    }

    // 获取风格样式的函数
    IEnumerator GetStyles()
    {
        string url = "https://api.deeparteffects.com/v1/noauth/styles";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("x-api-key", apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Styles fetched: " + request.downloadHandler.text);
            // 解析并展示风格
            StyleResponse styleResponse = JsonUtility.FromJson<StyleResponse>(request.downloadHandler.text);
            foreach (var style in styleResponse.styles)
            {
                Debug.Log("Style ID: " + style.id + ", Title: " + style.title);
            }
        }
        else
        {
            Debug.LogError("获取风格列表失败: " + request.error);
        }
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
        Debug.Log("Apply Style Button Clicked");

        if (string.IsNullOrEmpty(styleId))
        {
            Debug.LogError("Style ID is null or empty.");
            return;
        }

        if (capturedImage == null)
        {
            Debug.LogError("没有捕获的图像，无法应用风格。");
            return;
        }

        byte[] imageBytes = capturedImage.EncodeToJPG();
        string base64Image = System.Convert.ToBase64String(imageBytes);

        StartCoroutine(UploadImage(base64Image, styleId));
    }

    IEnumerator UploadImage(string base64Image, string styleId)
    {
        string url = "https://api.deeparteffects.com/v1/noauth/upload";

        // 构建 JSON 请求体
        string jsonRequestBody = "{\"styleId\":\"" + styleId + "\",\"imageBase64Encoded\":\"" + base64Image + "\",\"imageSize\":\"512\"}";

        byte[] postData = Encoding.UTF8.GetBytes(jsonRequestBody);
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(postData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("x-api-key", apiKey);

        Debug.Log("上传的请求体: " + jsonRequestBody);  // 输出构建的请求体

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("图像上传成功");
            DeepArtResponse response = JsonUtility.FromJson<DeepArtResponse>(request.downloadHandler.text);
            StartCoroutine(GetResult(response.submissionId));
        }
        else
        {
            Debug.LogError("图像上传失败: " + request.error);
            Debug.LogError("服务器响应: " + request.downloadHandler.text);
        }
    }

    IEnumerator GetResult(string submissionId)
{
    string url = $"https://api.deeparteffects.com/v1/noauth/result?submissionId={submissionId}";

    // 循环等待，直到状态为 finished
    while (true)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("x-api-key", apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Response: " + request.downloadHandler.text);
            DeepArtResult response = JsonUtility.FromJson<DeepArtResult>(request.downloadHandler.text);

            if (response.status == "finished")
            {
                string imageUrl = response.url;
                Debug.Log("Artwork is ready: " + imageUrl);
                StartCoroutine(DownloadImage(imageUrl));
                break; // 退出循环
            }
            else if (response.status == "error")
            {
                Debug.LogError("Artwork processing error.");
                break; // 处理失败，退出循环
            }
            else
            {
                Debug.Log("Artwork processing status: " + response.status);
                yield return new WaitForSeconds(5); // 等待5秒后重试
            }
        }
        else
        {
            Debug.LogError("Failed to get result: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);
            yield break; // 出现错误时退出
        }
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

// 风格列表响应类
[System.Serializable]
public class StyleResponse
{
    public Style[] styles;
}

[System.Serializable]
public class Style
{
    public string id;
    public string title;
}
