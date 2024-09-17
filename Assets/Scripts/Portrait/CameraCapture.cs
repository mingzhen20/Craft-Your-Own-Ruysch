using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class CameraCapture : MonoBehaviour
{
    WebCamTexture webCamTexture;
    public RawImage displayImage;       // 用于显示摄像头实时图像的UI组件
    public RawImage resultImageDisplay; // 用于显示风格化结果的UI组件
    private Texture2D capturedImage;    // 存储捕获的图片
    private bool hasCaptured = false;   // 标记是否已经拍摄

    void Start()
    {
        StartCamera(); // 初始化摄像头
    }

    // 开始摄像头
    void StartCamera()
    {
        webCamTexture = new WebCamTexture();
        displayImage.texture = webCamTexture;
        webCamTexture.Play();
        hasCaptured = false;
    }

    // 停止摄像头
    void StopCamera()
    {
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            webCamTexture.Stop();
        }
    }

    // 捕捉图片
    public void CaptureImage()
    {
        if (!hasCaptured)
        {
            // 捕获当前摄像头的画面
            capturedImage = new Texture2D(webCamTexture.width, webCamTexture.height);
            capturedImage.SetPixels(webCamTexture.GetPixels());
            capturedImage.Apply();

            // 更新显示的图片为捕获的静态图像
            displayImage.texture = capturedImage;

            // 停止摄像头预览
            StopCamera();

            hasCaptured = true; // 标记为已捕获
        }
        else
        {
            // 如果已经捕获过，重新启动摄像头以进行新的拍摄
            StartCamera();
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

        // 启动协程上传图片并应用风格
        StartCoroutine(UploadImage(capturedImage.EncodeToJPG()));
    }

    // 上传图片到DeepArt API并获取风格化后的图片
    IEnumerator UploadImage(byte[] imageBytes)
    {
        // 准备表单数据，添加图像文件
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", imageBytes, "image.jpg", "image/jpeg");

        // API URL 和 请求头
        string apiUrl = "https://api.deeparteffects.com/v1/noauth/upload";
        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl, form))
        {
            // 设置API密钥（注意：真实项目中应对密钥进行妥善保管）
            www.SetRequestHeader("x-api-key", "R55LYUGpVsaLPIXUwcrhJ4wrG2ZX5KdE46jzs3m4");

            // 发送请求并等待响应
            yield return www.SendWebRequest();

            // 处理请求结果
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("上传图片失败: " + www.error);
            }
            else
            {
                Debug.Log("风格应用成功！");

                // 获取响应的图片数据并显示风格化的图像
                Texture2D resultImage = new Texture2D(2, 2);
                resultImage.LoadImage(www.downloadHandler.data);
                resultImageDisplay.texture = resultImage; // 更新显示风格化后的图片
            }
        }
    }
}