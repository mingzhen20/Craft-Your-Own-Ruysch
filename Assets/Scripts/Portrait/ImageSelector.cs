using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // 添加这个引用以访问Image和Button组件

public class ImageSelector : MonoBehaviour
{
    public Image imageComponent;  // 图片的Image组件
    public bool isSelected = false;  // 记录图片是否被选中

    void Start()
    {
        // 如果图片有Button组件，绑定点击事件
        Button button = imageComponent.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(ToggleSelection);
        }
    }

    // 切换选中状态的方法
    void ToggleSelection()
    {
        isSelected = !isSelected;
        // 你可以根据选中状态改变图片的外观，比如边框颜色
        if (isSelected)
        {
            imageComponent.color = Color.green;  // 选中时变成绿色
        }
        else
        {
            imageComponent.color = Color.white;  // 未选中时恢复默认颜色
        }
    }
}
