using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtworkManager : MonoBehaviour
{
    public GameObject enlargedImageDisplay;
    public RectTransform scrollViewContent;
    public Transform flowerDisplayBar;
    public GameObject flowerButtonPrefab;
    public List<Button> allFlowerAreaButtons;
    private Image enlargedImageComponent;

    void Start()
    {

        enlargedImageComponent = enlargedImageDisplay.GetComponent<Image>();

        foreach (Transform imageTransform in scrollViewContent)
        {
            Button imageButton = imageTransform.GetComponent<Button>();
            Artwork artwork = imageTransform.GetComponent<Artwork>();
            if (imageButton != null && artwork != null)
            {
                imageButton.onClick.AddListener(() => OnArtworkClick(imageButton, artwork));
            }
            else
            {
                Debug.LogWarning("Button or Artwork component not found in: " + imageTransform.name);
            }
        }

    }

    void OnArtworkClick(Button clickedButton, Artwork artwork)
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

            foreach (RectTransform flowerArea in artwork.flowerAreas)
            {
                Button flowerButton = flowerArea.GetComponent<Button>();
                if (flowerButton == null)
                {
                    flowerButton = flowerArea.gameObject.AddComponent<Button>();
                }

                flowerButton.onClick.RemoveAllListeners();

                flowerButton.gameObject.SetActive(true);

                int index = System.Array.IndexOf(artwork.flowerAreas, flowerArea);
                flowerButton.onClick.AddListener(() => OnFlowerAreaClick(artwork.flowerSprites[index]));
            }
        }
        else
        {
            Debug.LogError("Image component not found on " + clickedButton.name);
        }
    }

    void OnFlowerAreaClick(Sprite flowerSprite)
    {
        Debug.Log("Clicked on flower area, flower: " + flowerSprite.name + " has been added to the display bar");

        GameObject flowerButton = Instantiate(flowerButtonPrefab, flowerDisplayBar);

        flowerButton.GetComponent<Image>().sprite = flowerSprite;

        RectTransform rectTransform = flowerButton.GetComponent<RectTransform>();
        rectTransform.localScale = Vector3.one; 
        rectTransform.anchoredPosition = Vector2.zero; 

        Debug.Log("Flower has been added to the DisplayBar: " + flowerSprite.name);
    }
}