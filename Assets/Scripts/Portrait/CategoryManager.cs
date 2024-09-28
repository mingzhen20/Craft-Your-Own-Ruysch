using UnityEngine;

public class CategoryManager : MonoBehaviour
{
    public GameObject headPanel;  // 发型分类内容的Panel
    public GameObject facePanel;  // 眼睛分类内容的Panel
    public GameObject skinColorPanel;  // 皮肤颜色分类内容的Panel

    // 当点击某个分类时显示该分类内容
    public void ShowCategory(string category)
    {
        // 隐藏所有分类内容
        HideAllCategories();

        // 根据点击的分类名称显示对应的内容
        switch (category)
        {
            case "Head":
                headPanel.SetActive(true);
                break;
            case "Face":
                facePanel.SetActive(true);
                break;
            case "SkinColor":
                skinColorPanel.SetActive(true);
                break;
                // 你可以继续添加更多的分类内容
        }
    }

    // 隐藏所有分类内容
    private void HideAllCategories()
    {
        headPanel.SetActive(false);
        facePanel.SetActive(false);
        skinColorPanel.SetActive(false);
    }
}
