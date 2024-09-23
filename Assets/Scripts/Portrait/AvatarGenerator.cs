using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class AvatarGenerator : MonoBehaviour
{
    public Image avatarImage;  // 用于显示avatar的UI Image组件
    private string baseUrl = "https://api.dicebear.com/9.x/micah/png";  // DiceBear API的基础URL
    private string currentSeed = "userSeed";  // avatar的种子

    // 默认参数
    private string hair = "short01";
    private string eyes = "round";

    // 点击不同UI元素时调用此方法来更新特定的发型
    public void UpdateHair(string hairStyle)
    {
        hair = hairStyle;  // 接收传递的发型参数
        GenerateAvatar();  // 更新avatar
    }

    // 点击不同UI元素时调用此方法来更新特定的眼睛样式
    public void UpdateEyes(string eyesStyle)
    {
        eyes = eyesStyle;  // 接收传递的眼睛参数
        GenerateAvatar();  // 更新avatar
    }

    // 生成avatar并更新显示
    private void GenerateAvatar()
    {
        // 构建包含发型和眼睛参数的API请求URL
        string url = $"{baseUrl}?seed={currentSeed}&hair={hair}&eyes={eyes}";
        StartCoroutine(LoadAvatar(url));
    }

    // 使用协程从API加载avatar并将其显示在UI中
    IEnumerator LoadAvatar(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(www);
            avatarImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }
}
