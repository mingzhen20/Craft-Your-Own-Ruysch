using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // 点击 "Create Avatar" 按钮时调用该方法
    public void LoadAvatarScene()
    {
        // 加载 Avatar 场景
        SceneManager.LoadScene("Scenes/Portrait/Avatar");
    }

    // 点击 "Take a Photo" 按钮时调用该方法
    public void LoadPhotoScene()
    {
        // 加载 Photo 场景
        SceneManager.LoadScene("Scenes/Portrait/Photo");
    }

    // 点击 "Generate Avatar" 按钮时调用该方法
    public void LoadAIScene()
    {
        // 加载 AI 场景
        SceneManager.LoadScene("Scenes/Portrait/AI");
    }
}
