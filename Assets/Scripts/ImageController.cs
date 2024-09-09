using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ImageManager : MonoBehaviour
{
    public float enlargedScale = 2.0f; 
    public float animationDuration = 0.5f; 
    private RectTransform currentImage; 
    private Vector3 originalPosition;
    private Vector3 originalScale;

    public void EnlargeImage(RectTransform imageToEnlarge)
    {
        if (currentImage != null)
        {
            ResetImage(); 
        }

        currentImage = imageToEnlarge;
        originalPosition = currentImage.localPosition;
        originalScale = currentImage.localScale;
        StartCoroutine(EnlargeImageCoroutine());
    }

   private IEnumerator EnlargeImageCoroutine()
   {
    float elapsedTime = 0;
    Vector3 targetScale = new Vector3(enlargedScale, enlargedScale, 1);
    
    RectTransform rectTransform = currentImage.GetComponent<RectTransform>();
    
    Vector2 enlargedAnchoredPosition = Vector2.zero; 

    while (elapsedTime < animationDuration)
    {
        rectTransform.anchoredPosition = Vector2.Lerp(originalPosition, enlargedAnchoredPosition, (elapsedTime / animationDuration));
        currentImage.localScale = Vector3.Lerp(originalScale, targetScale, (elapsedTime / animationDuration));
        
        elapsedTime += Time.deltaTime;
        yield return null;
    }

    rectTransform.anchoredPosition = enlargedAnchoredPosition; 
    currentImage.localScale = targetScale;  
    }

    public void ResetImage()
    {
        if (currentImage != null)
        {
            currentImage.localPosition = originalPosition;
            currentImage.localScale = originalScale;
            currentImage = null;
        }
    }
}