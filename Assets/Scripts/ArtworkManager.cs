using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ArtworkManager : MonoBehaviour
{
    public GameObject enlargedImageDisplay;  
    public RectTransform scrollViewContent;  
    private Image enlargedImageComponent;    

    void Start()
    {
        enlargedImageComponent = enlargedImageDisplay.GetComponent<Image>();

        foreach (Transform imageTransform in scrollViewContent)
        {
            Button imageButton = imageTransform.GetComponent<Button>();
            if (imageButton != null)
            {
                imageButton.onClick.AddListener(() => OnImageClick(imageButton));
            }
        }
    }

    void OnImageClick(Button clickedButton)
    {
        
        Image clickedImage = clickedButton.GetComponent<Image>();

        if (clickedImage != null)
        {
            
            enlargedImageComponent.sprite = clickedImage.sprite;

            float originalWidth = clickedImage.sprite.texture.width;
            float originalHeight = clickedImage.sprite.texture.height;
            float aspectRatio = originalWidth / originalHeight;

            float displayHeight = 500f; 
            float displayWidth = displayHeight * aspectRatio; 

            enlargedImageComponent.rectTransform.sizeDelta = new Vector2(displayWidth, displayHeight);

            enlargedImageComponent.rectTransform.anchoredPosition = Vector2.zero;

            enlargedImageDisplay.SetActive(true);
        }
    }
}