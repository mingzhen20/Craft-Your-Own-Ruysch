using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class AvatarGenerator : MonoBehaviour
{
    public Image avatarImage;  // 显示avatar的UI Image组件
    public int avatarSize;  // 头像的大小
    private string baseUrl = "https://api.dicebear.com/9.x/open-peeps/png";  // Open Peeps API的基础URL
    private string currentSeed = "userSeed";  // 用户选择的头像种子

    public Button confirmButton;

    // 默认值（可以根据用户选择进行更新）
    public string skinColor;  // 表情
    public string head;  // 头部样式
    public string face;  // 脸部样式
    public int maskProbability;  // 戴口罩的概率
    public int facialHairProbability;  // 有胡须的概率
    public string facialHair;  // 胡须样式
    public string accessories;  // 配饰
    public int accessoriesProbability;  // 有配饰的概率



    void Start()
    {
        // 在游戏开始时加载默认头像
        GenerateAvatar();
        confirmButton.onClick.AddListener(OnConfirmButtonClick);
    }

    // 更新表情
    public void UpdateSkinColor(string newSkinColor)
    {
        skinColor = newSkinColor;
        GenerateAvatar();  // 每次更新都重新生成头像
    }

    public void UpdateHead(string newHead)
    {
        head = newHead;
        GenerateAvatar();
    }

    public void UpdateFace(string newFace)
    {
        face = newFace;
        GenerateAvatar();
    }

    public void UpdateMaskProbability(float newMaskProbability)
    {
        maskProbability = (int)newMaskProbability;
        GenerateAvatar();
    }

    public void UpdateFacialHairProbability(float newFacialHairProbability)
    {
        facialHairProbability = (int)newFacialHairProbability;
        GenerateAvatar();
    }

    public void UpdateFacialHair(string newFacialHair)
    {
        facialHairProbability = 100;
        facialHair = newFacialHair;
        GenerateAvatar();
    }

    public void UpdateAccessories(string newAccessories)
    {
        accessoriesProbability = 100;
        accessories = newAccessories;
        GenerateAvatar();
    }

    public void UpdateAccessoriesProbability(float newAccessoriesProbability)
    {
        accessoriesProbability = (int)newAccessoriesProbability;
        GenerateAvatar();
    }

    // 生成头像的主方法
    private void GenerateAvatar()
    {
        // 构建包含用户选择的API URL，添加配饰和表情等参数
        string url = $"{baseUrl}?seed={currentSeed}&flip=true&skinColor={skinColor}&face={face}&size={avatarSize}&maskProbability={maskProbability}&head={head}&facialHairProbability={facialHairProbability}&accessoriesProbability={accessoriesProbability}&facialHair={facialHair}&accessories={accessories}";
        StartCoroutine(LoadAvatar(url));  // 使用协程从API加载头像
    }

    // 从API加载头像的协程
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

            // Save the texture data to PlayerPrefs
            SaveImageForEndPage(texture);
        }
    }

    // 保存图片数据
    void SaveImageForEndPage(Texture2D texture)
    {
        byte[] imageBytes = texture.EncodeToPNG(); // Encode the texture into PNG
        string encodedImage = Convert.ToBase64String(imageBytes);
        PlayerPrefs.SetString("FinalImage", encodedImage);
        PlayerPrefs.Save();
    }


    public void OnConfirmButtonClick()
    {
        SceneManager.LoadScene("EndPage"); // 确保场景名称是正确的
    }
}
