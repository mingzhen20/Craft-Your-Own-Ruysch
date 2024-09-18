using System;
using UnityEngine;
using UnityEngine.UI;

public class ArtworkManager : MonoBehaviour
{
    public GameObject enlargedImageDisplay;        
    public RectTransform scrollViewContent;        
    public Transform flowerDisplayBar;             
    public GameObject flowerButtonPrefab;          // 花朵按钮的预制体
    public GameObject fullScreen3DView;            
    public Transform flower3DViewTransform;        // 放置3D花朵模型的容器
    public Button close3DViewButton;              
    public Button endFlowerSelectionButton;       
    public GameObject flowerArrangementArea;       
    public Transform upper3DFlowerTransform;       // 3D花朵显示在页面顶部的位置
    private Image enlargedImageComponent;          
    private GameObject current3DFlower;           
    private GameObject currentArtwork;             
    private bool isFlowerArrangementPhase = false; 

    void Start()
    {
        enlargedImageComponent = enlargedImageDisplay.GetComponent<Image>();

        // 为 ScrollView 中的画作绑定点击事件
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

        // 初始化插花区域和全屏3D视图，初始时隐藏
        fullScreen3DView.SetActive(false);
        flowerArrangementArea.SetActive(false);

        endFlowerSelectionButton.onClick.AddListener(OnEndFlowerSelection);

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

    // 点击画作时显示画作并绑定花朵区域点击事件
    void OnArtworkClick(Button clickedButton, Artwork artwork)
    {
        Image clickedImage = clickedButton.GetComponent<Image>();

        if (clickedImage != null)
        {
            enlargedImageComponent.sprite = clickedImage.sprite;

            float aspectRatio = (float)clickedImage.sprite.texture.width / clickedImage.sprite.texture.height;
            float displayHeight = 500f;
            float displayWidth = displayHeight * aspectRatio;

            enlargedImageComponent.rectTransform.sizeDelta = new Vector2(displayWidth, displayHeight);
            enlargedImageComponent.rectTransform.anchoredPosition = Vector2.zero;

            enlargedImageDisplay.SetActive(true);

            // 禁用之前画作的花朵区域
            if (currentArtwork != null)
            {
                DisableFlowerAreas(currentArtwork);
            }

            currentArtwork = clickedButton.gameObject;
            EnableFlowerAreas(artwork);

            // 为每个花朵区域绑定点击事件
            for (int i = 0; i < artwork.flowerAreas.Length; i++)
            {
                RectTransform flowerArea = artwork.flowerAreas[i];
                Button flowerButton = flowerArea.GetComponent<Button>();
                if (flowerButton == null)
                {
                    flowerButton = flowerArea.gameObject.AddComponent<Button>();
                }

                flowerButton.onClick.RemoveAllListeners();
                flowerButton.gameObject.SetActive(true);

                int index = i; 
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

    void EnableFlowerAreas(Artwork artwork)
    {
        foreach (RectTransform flowerArea in artwork.flowerAreas)
        {
            flowerArea.gameObject.SetActive(true);
        }
    }

    void DisableFlowerAreas(GameObject previousArtwork)
    {
        Artwork previousArtworkScript = previousArtwork.GetComponent<Artwork>();
        if (previousArtworkScript != null)
        {
            foreach (RectTransform flowerArea in previousArtworkScript.flowerAreas)
            {
                flowerArea.gameObject.SetActive(false);
            }
        }
    }

    void OnFlowerBarClick(GameObject flower3DModel)
    {
        if (isFlowerArrangementPhase)
        {
            Show3DFlowerAtTop(flower3DModel);
        }
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

    void OnEndFlowerSelection()
    {
        enlargedImageDisplay.SetActive(false);
        scrollViewContent.gameObject.SetActive(false);
        flowerArrangementArea.SetActive(true);
        flowerDisplayBar.gameObject.SetActive(true);

        if (endFlowerSelectionButton != null)
        {
            endFlowerSelectionButton.gameObject.SetActive(false);
        }

        isFlowerArrangementPhase = true;
        BindFlowerBarClickEvents();

        Debug.Log("Entered flower arrangement phase");
    }

    void BindFlowerBarClickEvents()
    {
        foreach (Transform flowerButton in flowerDisplayBar)
        {
            Button button = flowerButton.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                GameObject flower3DModel = Get3DModelForFlower(button);
                button.onClick.AddListener(() => OnFlowerBarClick(flower3DModel));
            }
        }
    }

    GameObject Get3DModelForFlower(Button button)
    {
        string buttonName = button.name.Replace("(Clone)", "").Trim();
        Debug.Log("Button name after cleaning: " + buttonName);

        string path = "3DModels/" + buttonName;
        Debug.Log("Loading 3D model from path: " + path);

        GameObject flower3DModel = Resources.Load<GameObject>(path);

        if (flower3DModel == null)
        {
            Debug.LogError("Failed to load 3D model for: " + buttonName);
        }
        else
        {
            Debug.Log("Successfully loaded 3D model for: " + buttonName);
        }

        return flower3DModel;
    }
    void Show3DFlowerAtTop(GameObject flower3DModel)
    {
        if (current3DFlower != null)
        {
            Destroy(current3DFlower);
        }

        current3DFlower = Instantiate(flower3DModel, upper3DFlowerTransform);

        if (current3DFlower != null)
        {
            Debug.Log("Successfully loaded 3D model: " + current3DFlower.name);

            current3DFlower.transform.localScale = Vector3.one;
            current3DFlower.transform.localPosition = Vector3.zero;
            current3DFlower.transform.localRotation = Quaternion.identity;
        }
        else
        {
            Debug.LogError("Failed to load 3D model");
        }
    }
}