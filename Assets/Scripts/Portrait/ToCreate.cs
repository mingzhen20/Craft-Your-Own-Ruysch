using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // 这个方法将在按钮点击时调用
    public void LoadAvatarScene()
    {
        // 加载Avatar场景
        SceneManager.LoadScene("Scenes/Portrait/Avatar");
    }
}
