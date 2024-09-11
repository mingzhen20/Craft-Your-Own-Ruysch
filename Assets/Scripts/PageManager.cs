using UnityEngine;
using UnityEngine.SceneManagement;

public class PageManager : MonoBehaviour
{
    public void LoadNextPage()
    {
        SceneManager.LoadScene("GalleryScene");
    }
}