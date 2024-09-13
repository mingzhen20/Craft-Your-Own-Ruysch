using UnityEngine;
using UnityEngine.UI;

public class ArtworkManager : MonoBehaviour
{
    public GameObject enlargedImageDisplay;   
    public RectTransform scrollViewContent;   
    public Transform flowerDisplayBar;        
    public GameObject flowerButtonPrefab;     
    public GameObject fullScreen3DView;       
    public Transform flower3DViewTransform;   
    public Button close3DViewButton;         
    private Image enlargedImageComponent;     
    private GameObject current3DFlower;       

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
                Debug.LogWarning("Button or Artwork component not found on: " + imageTransform.name);
            }
        }

        fullScreen3DView.SetActive(false);
        if (close3DViewButton != null)
        {
            close3DViewButton.gameObject.SetActive(false);  
            close3DViewButton.onClick.AddListener(HideFullScreen3DView);
        }
        else
        {
            Debug.LogWarning("Close button not set");
        }
    }

    void OnArtworkClick(Button clickedButton, Artwork artwork)
    {
        Image clickedImage = clickedButton.GetComponent<Image>();

        if (clickedImage != null)
        {
            // Display the selected artwork
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
                flowerButton.onClick.AddListener(() => OnFlowerAreaClick(artwork.flowerSprites[index], artwork.flower3DModels[index]));
            }
        }
        else
        {
            Debug.LogError("Image component not found on " + clickedButton.name);
        }
    }

    void OnFlowerAreaClick(Sprite flowerSprite, GameObject flower3DModel)
    {
        Show2DFlower(flowerSprite);

        ShowFullScreen3DFlower(flower3DModel);
    }

    void Show2DFlower(Sprite flowerSprite)
    {
        GameObject flowerButton = Instantiate(flowerButtonPrefab, flowerDisplayBar);
        flowerButton.GetComponent<Image>().sprite = flowerSprite;

        Debug.Log("2D flower displayed: " + flowerSprite.name);
    }

    void ShowFullScreen3DFlower(GameObject flower3DModel)
    {
        if (current3DFlower != null)
        {
            Destroy(current3DFlower);
            Debug.Log("Destroyed currently displayed 3D flower: " + current3DFlower.name);
        }

        fullScreen3DView.SetActive(true);

        close3DViewButton.gameObject.SetActive(true);

        current3DFlower = Instantiate(flower3DModel, flower3DViewTransform);

        if (current3DFlower != null)
        {
            Debug.Log("Successfully instantiated 3D model: " + current3DFlower.name);
        }
        else
        {
            Debug.LogError("Failed to instantiate 3D model!");
        }

        current3DFlower.transform.localScale = Vector3.one;
        current3DFlower.transform.localPosition = Vector3.zero;
        current3DFlower.transform.localRotation = Quaternion.identity;

        AutoRotate autoRotate = current3DFlower.AddComponent<AutoRotate>();
    }

    public void HideFullScreen3DView()
    {
        if (current3DFlower != null)
        {
            Destroy(current3DFlower);
        }

        fullScreen3DView.SetActive(false);

        close3DViewButton.gameObject.SetActive(false);

        enlargedImageDisplay.SetActive(true);  
        Debug.Log("Closed full-screen 3D view, returning to flower selection");
    }
}