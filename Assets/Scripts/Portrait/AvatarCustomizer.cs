using TMPro; // 引入TextMeshPro的命名空间
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using Unity.VectorGraphics; // 引入VectorGraphics命名空间
using System.IO;


public class AvatarCustomizer : MonoBehaviour
{
    public Image avatarImage; // 显示头像的UI Image组件
    public TMP_Dropdown topTypeDropdown; // 发型选择
    public TMP_Dropdown hairColorDropdown; // 发色选择
    public TMP_Dropdown clotheTypeDropdown; // 服装选择

    private string apiUrl = "https://avataaars.io/";
    public void GenerateAvatar()
    {
        // 获取用户选择的参数
        string topType = topTypeDropdown.options[topTypeDropdown.value].text;
        string hairColor = hairColorDropdown.options[hairColorDropdown.value].text;
        string clotheType = clotheTypeDropdown.options[clotheTypeDropdown.value].text;

        // 构建API参数字符串，将 avatarStyle 设置为 Transparent
        string avatarParameters = $"?avatarStyle=Transparent&topType={topType}&hairColor={hairColor}&clotheType={clotheType}";

        // 开始加载头像
        StartCoroutine(LoadAvatar(apiUrl + avatarParameters));
    }

    IEnumerator LoadAvatar(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url); // 这里改为获取Text数据
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error fetching avatar: " + request.error);
        }
        else
        {
            // 获取下载到的SVG数据
            string svgText = request.downloadHandler.text;

            // 将SVG转化为Sprite
            var sceneInfo = SVGParser.ImportSVG(new StringReader(svgText));
            var options = new VectorUtils.TessellationOptions()
            {
                StepDistance = 100f,
                MaxCordDeviation = 0.5f,
                MaxTanAngleDeviation = 0.1f,
                SamplingStepSize = 0.01f
            };

            // 生成Sprite
            var geometry = VectorUtils.TessellateScene(sceneInfo.Scene, options);
            var sprite = VectorUtils.BuildSprite(geometry, 100.0f, VectorUtils.Alignment.Center, Vector2.zero, 128, true);

            // 显示生成的Sprite
            avatarImage.sprite = sprite;
        }
    }
}
