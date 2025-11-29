using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [Header("Contenido de la Historia")]
    public Sprite[] storyImages; 
    [TextArea(3, 10)]
    public string[] storyTexts; 

    [Header("Referencias de UI")]
    public Image displayImage;
    public TextMeshProUGUI displayText;
    public Button nextButton;
    public TextMeshProUGUI nextButtonText;

    [Header("Audio del Tutorial")]
    public AudioSource audioSource;
    public AudioClip tutorialMusic;

    private int currentIndex = 0;
    public string firstLevelName = "Nivel_1";

    void Start()
    {
        UpdateUI();
        
        nextButton.onClick.AddListener(NextSlide);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)    
        {
            Debug.LogError("TutorialManager requiere un AudioSource para la música de fondo.");
        }
    }

    void UpdateUI()
    {
        if (currentIndex < storyImages.Length) 
            displayImage.sprite = storyImages[currentIndex];
        
        if (currentIndex < storyTexts.Length) 
            displayText.text = storyTexts[currentIndex];

        if (currentIndex == storyImages.Length - 1)
        {
            nextButtonText.text = "¡Jugar!";
        }
        else
        {
            nextButtonText.text = "Siguiente";
        }
    }

    public void NextSlide()
    {
        currentIndex++;

        if (currentIndex >= storyImages.Length)
        {
            StartGame();
        }
        else
        {
            UpdateUI();
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(firstLevelName);
    }
}