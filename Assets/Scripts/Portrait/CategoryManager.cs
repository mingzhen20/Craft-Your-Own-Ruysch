using UnityEngine;

public class CategoryManager : MonoBehaviour
{
    // 分别对应每个分类的 Scroll View
    public GameObject headScrollView;   // 发型分类的Scroll View
    public GameObject faceScrollView;   // 眼睛分类的Scroll View
    public GameObject skinColorScrollView;  // 皮肤颜色分类的Scroll View
    public GameObject facialHairScrollView;  // 胡须分类的Scroll View
    public GameObject accessoriesScrollView;  // 配饰分类的Scroll View

    // 当点击某个分类时显示对应的 Scroll View
    public void ShowCategory(string category)
    {
        // 隐藏所有分类的 Scroll View
        HideAllScrollViews();

        // 根据点击的分类名称显示对应的 Scroll View
        switch (category)
        {
            case "Head":
                headScrollView.SetActive(true);
                break;
            case "Face":
                faceScrollView.SetActive(true);
                break;
            case "SkinColor":
                skinColorScrollView.SetActive(true);
                break;
            case "FacialHair":
                facialHairScrollView.SetActive(true);
                break;
            case "Accessories":
                accessoriesScrollView.SetActive(true);
                break;
        }
    }

    // 隐藏所有的 Scroll View
    private void HideAllScrollViews()
    {
        headScrollView.SetActive(false);
        faceScrollView.SetActive(false);
        skinColorScrollView.SetActive(false);
        facialHairScrollView.SetActive(false);
        accessoriesScrollView.SetActive(false);
    }
}
