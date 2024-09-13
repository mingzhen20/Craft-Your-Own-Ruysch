using UnityEngine;
using UnityEngine.UI;

public class FlowerDisplay : MonoBehaviour
{
    public Transform flowerDisplayBar;  // 花朵展示栏
    public GameObject flowerButtonPrefab;  // 预制体，用于显示花朵的按钮

    // 选择的画作
    private Artwork currentArtwork;

    // 当用户点击某幅画作时调用此方法
    public void DisplayFlowers(Artwork selectedArtwork)
    {
        currentArtwork = selectedArtwork;

        // 清空之前的花朵展示
        foreach (Transform child in flowerDisplayBar)
        {
            Destroy(child.gameObject);
        }

        // 遍历画作中的花朵，并在展示栏中生成按钮
        foreach (Sprite flower in selectedArtwork.flowerSprites)
        {
            // 实例化一个按钮
            GameObject flowerButton = Instantiate(flowerButtonPrefab, flowerDisplayBar);
            flowerButton.GetComponent<Image>().sprite = flower;

            // 为每个按钮添加点击事件，点击后显示对应的3D模型
            flowerButton.GetComponent<Button>().onClick.AddListener(() => ShowFlowerIn3D(selectedArtwork, flower));
        }
    }

    // 显示3D花朵模型
    private void ShowFlowerIn3D(Artwork artwork, Sprite flower)
    {
        // 查找花朵的索引，找到对应的3D模型
        int flowerIndex = System.Array.IndexOf(artwork.flowerSprites, flower);
        if (flowerIndex >= 0 && flowerIndex < artwork.flowerModels.Length)
        {
            GameObject flower3DModel = artwork.flowerModels[flowerIndex];
            flower3DModel.SetActive(true);  // 显示3D模型
            // 这里可以添加额外的逻辑来全屏显示3D模型
        }
    }
}