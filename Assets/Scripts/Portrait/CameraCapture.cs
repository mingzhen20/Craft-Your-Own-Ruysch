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
    public Button confirmButton;

    void Start()
    {
        StartCamera();
        getStylesButton.onClick.AddListener(OnGetStylesButtonClicked);
        confirmButton.onClick.AddListener(ConfirmSelection);
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

        string jsonRequestBody = "{\"styleId\":\"" + styleId + "\",\"imageBase64Encoded\":\"" + base64Image + "\",\"imageSize\":\"512\"}";
        byte[] postData = Encoding.UTF8.GetBytes(jsonRequestBody);
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(postData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("x-api-key", apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            DeepArtResponse response = JsonUtility.FromJson<DeepArtResponse>(request.downloadHandler.text);
            StartCoroutine(GetResult(response.submissionId));
        }
        else
        {
            Debug.LogError("图像上传失败: " + request.error);
        }
    }

    IEnumerator GetResult(string submissionId)
    {
        string url = $"https://api.deeparteffects.com/v1/noauth/result?submissionId={submissionId}";

        while (true)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.SetRequestHeader("x-api-key", apiKey);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                DeepArtResult response = JsonUtility.FromJson<DeepArtResult>(request.downloadHandler.text);

                if (response.status == "finished")
                {
                    StartCoroutine(DownloadImage(response.url));
                    break;
                }
                else if (response.status == "error")
                {
                    Debug.LogError("风格化处理错误。");
                    break;
                }
                else
                {
                    yield return new WaitForSeconds(5);
                }
            }
            else
            {
                Debug.LogError("获取结果失败: " + request.error);
                yield break;
            }
        }
    }

    IEnumerator DownloadImage(string imageUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D styledTexture = DownloadHandlerTexture.GetContent(request);
            resultImageDisplay.texture = styledTexture;

            // 保存图片数据到 PlayerPrefs
            byte[] imageBytes = styledTexture.EncodeToPNG();
            PlayerPrefs.SetString("FinalImage", System.Convert.ToBase64String(imageBytes));

            Debug.Log("风格化图片下载并显示成功。");
        }
        else
        {
            Debug.LogError("下载风格化图像失败: " + request.error);
        }
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
