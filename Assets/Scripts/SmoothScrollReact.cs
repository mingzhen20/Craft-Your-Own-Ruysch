using UnityEngine;
using UnityEngine.UI;

public class SmoothScrollReact : MonoBehaviour
{
    public ScrollRect scrollRect; 
    public float scrollSpeed = 0.005f; 

    void Update()
    {

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                scrollRect.horizontalNormalizedPosition -= touch.deltaPosition.x * scrollSpeed * Time.deltaTime;
            }
        }
    }
}

