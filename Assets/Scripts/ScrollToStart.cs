using UnityEngine;
using UnityEngine.UI;

public class ScrollToStart : MonoBehaviour
{
    public ScrollRect scrollRect;
    void Start()
    {
        scrollRect.horizontalNormalizedPosition = 0f;
    }
}
