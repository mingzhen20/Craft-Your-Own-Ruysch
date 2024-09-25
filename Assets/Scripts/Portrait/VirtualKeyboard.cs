using UnityEngine;
using TMPro; // 需要引入TextMeshPro的命名空间

public class VirtualKeyboard : MonoBehaviour
{
    public TMP_InputField inputField; // 使用TMP的输入框
    private TouchScreenKeyboard keyboard;

    void Update()
    {
        // 如果输入框被选中且键盘未打开
        if (inputField.isFocused && keyboard == null)
        {
            keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
            Debug.Log("TouchScreenKeyboard opened."); // 添加这行代码用于调试
        }


        // 如果键盘已打开
        if (keyboard != null)
        {
            // 将键盘输入的文本设置为输入框的文本
            inputField.text = keyboard.text;

            // 检查键盘是否关闭
            if (keyboard.status == TouchScreenKeyboard.Status.Done || keyboard.status == TouchScreenKeyboard.Status.Canceled)
            {
                keyboard = null; // 键盘关闭
            }
        }
    }
}
