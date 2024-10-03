using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Unity.VectorGraphics;
using System.Collections;
using System.IO;
using TMPro;

public class AvatarCustomizer : MonoBehaviour
{
    public RawImage avatarDisplay;  // 用于显示头像的RawImage组件
    public TMP_Dropdown topTypeDropdown;  // 发型TMP_Dropdown
    public TMP_Dropdown hairColorDropdown;  // 发色TMP_Dropdown
    public TMP_Dropdown clotheTypeDropdown;  // 服装TMP_Dropdown
    public Button generateAvatarButton;  // 生成头像的按钮

    private string apiUrl = "https://avataaars.io/";

    void Start()
    {
        // 监听按钮点击事件
        generateAvatarButton.onClick.AddListener(OnGenerateAvatarClicked);
    }

    void OnGenerateAvatarClicked()
    {
        // 获取用户选择的选项
        string topType = topTypeDropdown.options[topTypeDropdown.value].text;
        string hairColor = hairColorDropdown.options[hairColorDropdown.value].text;
        string clotheType = clotheTypeDropdown.options[clotheTypeDropdown.value].text;

        // 生成头像
        GenerateAvatar(topType, hairColor, clotheType);
    }

    public void GenerateAvatar(string topType, string hairColor, string clotheType)
    {
        // 构建Avataaars API URL, 使用 PNG 格式
        string avatarUrl = $"{apiUrl}png?avatarStyle=Circle&topType={topType}&hairColor={hairColor}&clotheType={clotheType}&size=512";

        Debug.Log($"Generating avatar with URL: {avatarUrl}");  // 调试信息1：打印URL
        // 启动协程下载PNG头像
        StartCoroutine(DownloadImage(avatarUrl));
    }

    IEnumerator DownloadImage(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Accept", "image/png");
        Debug.Log("Sending request to URL: " + url);  // 调试信息2：确认请求已发送

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error downloading image: " + request.error);
        }
        else
        {
            Debug.Log("Download successful, processing image.");  // 调试信息4：确认下载成功
            // 尝试将下载的数据转换为Texture
            try
            {
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(request.downloadHandler.data);
                avatarDisplay.texture = texture;
                Debug.Log("Image processed and displayed successfully.");  // 调试信息5：图像处理成功
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error processing image data: " + ex.Message);
            }
        }
    }

}
