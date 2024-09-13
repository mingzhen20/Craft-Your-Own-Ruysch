using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class CameraCapture : MonoBehaviour
{
    WebCamTexture webCamTexture;
    public RawImage displayImage; // 显示摄像头实时图像
    public RawImage resultImageDisplay; // 显示风格化后的结果
    private Texture2D capturedImage; // 存储捕获的图片

    void Start()
    {
        webCamTexture = new WebCamTexture();
        displayImage.texture = webCamTexture;
        webCamTexture.Play();
    }

    public void CaptureImage()
    {
        capturedImage = new Texture2D(webCamTexture.width, webCamTexture.height);
        capturedImage.SetPixels(webCamTexture.GetPixels());
        capturedImage.Apply();
        displayImage.texture = capturedImage; // 更新UI显示捕获的图片
    }

    public void ApplyArtStyle()
    {
        if (capturedImage == null)
        {
            Debug.LogError("No image captured to apply style.");
            return;
        }

        StartCoroutine(UploadImage(capturedImage.EncodeToJPG()));
    }

    IEnumerator UploadImage(byte[] imageBytes)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", imageBytes, "image.jpg", "image/jpeg");

        string apiUrl = "https://api.deeparteffects.com/v1/noauth/upload";
        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl, form))
        {
            www.SetRequestHeader("x-api-key", "R55LYUGpVsaLPIXUwcrhJ4wrG2ZX5KdE46jzs3m4");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                Debug.Log("Style applied successfully!");

                Texture2D resultImage = new Texture2D(2, 2);
                resultImage.LoadImage(www.downloadHandler.data);
                resultImageDisplay.texture = resultImage;
            }
        }
    }
}
