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
        // 构建Avataaars API URL, 使用 SVG 格式
        string avatarUrl = $"{apiUrl}?avatarStyle=Transparent&topType={topType}&hairColor={hairColor}&clotheType={clotheType}&format=svg";

        // 启动协程加载SVG头像
        StartCoroutine(LoadSVG(avatarUrl));
    }

    IEnumerator LoadSVG(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error fetching SVG: " + request.error);
        }
        else
        {
            // 获取下载到的SVG数据
            string svgText = request.downloadHandler.text;

            // 将SVG数据解析为场景
            var sceneInfo = SVGParser.ImportSVG(new StringReader(svgText));

            // 创建RenderTexture用于渲染
            int textureWidth = 512;  // 可以根据需要调整
            int textureHeight = 512;
            RenderTexture renderTexture = new RenderTexture(textureWidth, textureHeight, 24);

            // 渲染SVG到Texture
            RenderSVGToTexture(sceneInfo, renderTexture);

            // 将RenderTexture转换为Texture2D
            Texture2D avatarTexture = ConvertRenderTextureToTexture2D(renderTexture);

            // 显示Texture2D到RawImage
            avatarDisplay.texture = avatarTexture;

            // 释放RenderTexture
            renderTexture.Release();
        }
    }

    // 渲染SVG到RenderTexture
    void RenderSVGToTexture(SVGParser.SceneInfo sceneInfo, RenderTexture renderTexture)
    {
        // 设置渲染目标为RenderTexture
        RenderTexture.active = renderTexture;

        // 清除当前渲染目标
        GL.Clear(true, true, Color.clear);

        // 设置TessellationOptions
        var tessOptions = new VectorUtils.TessellationOptions
        {
            StepDistance = 100f,
            MaxCordDeviation = 0.5f,
            MaxTanAngleDeviation = 0.1f,
            SamplingStepSize = 0.01f
        };

        // 生成矢量几何
        var geometry = VectorUtils.TessellateScene(sceneInfo.Scene, tessOptions);

        // 生成SVG的Sprite
        var sprite = VectorUtils.BuildSprite(geometry, 1.0f, VectorUtils.Alignment.Center, Vector2.zero, 128, true);

        // 创建材质并设置主纹理为生成的Sprite
        Material material = new Material(Shader.Find("Sprites/Default"));
        material.mainTexture = sprite.texture;

        // 开始绘制SVG
        Graphics.Blit(sprite.texture, renderTexture);

        // 解除当前RenderTexture的绑定
        RenderTexture.active = null;
    }

    // 将RenderTexture转换为Texture2D
    Texture2D ConvertRenderTextureToTexture2D(RenderTexture renderTexture)
    {
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);

        // 将RenderTexture的内容读取到Texture2D
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        // 解除当前RenderTexture的绑定
        RenderTexture.active = null;

        return texture;
    }
}
