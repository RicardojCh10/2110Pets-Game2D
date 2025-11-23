using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [Header("Contenido de la Historia")]
    public Sprite[] storyImages; // Arrastra tus imágenes aquí
    [TextArea(3, 10)]
    public string[] storyTexts; // Escribe tus textos aquí

    [Header("Referencias de UI")]
    public Image displayImage;
    public TextMeshProUGUI displayText;
    public Button nextButton;
    public TextMeshProUGUI nextButtonText; // Para cambiar "Siguiente" a "Jugar"

    // Variables de Audio
    [Header("Audio del Tutorial")]
    public AudioSource audioSource;
    public AudioClip tutorialMusic;

    private int currentIndex = 0;
    public string firstLevelName = "Nivel_1"; // El nombre de tu primer nivel

    void Start()
    {
        UpdateUI();
        
        // Conectar botón "Siguiente"
        nextButton.onClick.AddListener(NextSlide);

        // obtener el AudioSource para la música al inicio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)    
        {
            Debug.LogError("TutorialManager requiere un AudioSource para la música de fondo.");
        }
    }

    void UpdateUI()
    {
        // Actualiza imagen y texto
        if (currentIndex < storyImages.Length) 
            displayImage.sprite = storyImages[currentIndex];
        
        if (currentIndex < storyTexts.Length) 
            displayText.text = storyTexts[currentIndex];

        // Si es la última diapositiva, cambia el texto del botón
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
            // Si ya no hay más imágenes, carga el juego
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