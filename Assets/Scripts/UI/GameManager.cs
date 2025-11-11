using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // --- NUEVO: Para controlar el PlayerInput

public class GameManager : MonoBehaviour
{
    // --- Singleton Pattern ---
    public static GameManager Instance;

    [Header("Referencias de la UI")]
    public GameObject hudCanvas;
    public Slider healthSlider;
    public TextMeshProUGUI scoreText;
    public GameObject gameOverPanel;
    public Slider kiroHealthSlider;
    public GameObject levelCompletePanel; // --- NUEVO ---

    [Header("Referencias del Jugador")]
    public PlayerInput playerInput; // --- NUEVO: Arrastra el componente "Player Input" de Aiden aquí

    [Header("Variables del Jugador")]
    public int maxPlayerHealth = 100;
    private int currentPlayerHealth;
    private int currentScore = 0;

    [Header("Variables de Kiro")]
    public float maxKiroHealth = 60f;
    public float healthDrainPerSecond = 1f;
    private float currentKiroHealth;

    // --- NUEVO: Nombres de Escenas (¡Asegúrate de que coincidan!) ---
    [Header("Configuración de Nivel")]
    public string nextLevelName = "Nivel_2";
    public string mainMenuSceneName = "MainMenu";


    private void Awake()
    {
        // ... (Tu código de Awake no cambia) ...
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
            DontDestroyOnLoad(hudCanvas); 
        }
        else
        {
            Destroy(gameObject);
            Destroy(hudCanvas);
        }
    }

    void Start()
    {
        // --- NUEVO: Habilitar el input al inicio ---
        if (playerInput != null)
        {
            playerInput.ActivateInput();
        }
        Time.timeScale = 1f; // Nos aseguramos de que el juego no esté pausado

        // ... (El resto de tu código de Start no cambia) ...
        currentPlayerHealth = maxPlayerHealth;
        healthSlider.maxValue = maxPlayerHealth;
        healthSlider.value = currentPlayerHealth;
        scoreText.text = "Monedas: 0";
        gameOverPanel.SetActive(false); 
        levelCompletePanel.SetActive(false); // --- NUEVO: Asegurarse de que esté apagado

        if (kiroHealthSlider != null)
        {
            currentKiroHealth = maxKiroHealth;
            kiroHealthSlider.maxValue = maxKiroHealth;
            kiroHealthSlider.value = currentKiroHealth;
        }
    }

    void Update()
    {
        // ... (Tu código de Update para Kiro no cambia) ...
        if (kiroHealthSlider != null && currentKiroHealth > 0)
        {
            currentKiroHealth -= healthDrainPerSecond * Time.deltaTime;
            kiroHealthSlider.value = currentKiroHealth; 
            if (currentKiroHealth <= 0)
            {
                KiroDies();
            }
        }
    }

    // --- NUEVO: Función principal para Nivel Completado ---
    public void CompleteLevel()
    {
        if (playerInput != null)
        {
            playerInput.DeactivateInput(); // Desactiva el control del jugador
        }
        Time.timeScale = 0f; // Pausa el juego
        levelCompletePanel.SetActive(true); // Muestra el panel de victoria
    }

    // --- Funciones de Botones (Reutilizaremos algunas) ---

    // Esta función la usan AMBOS paneles (Game Over y Level Complete)
    public void OnRetryButton()
    {
        Time.timeScale = 1f;
        
        // Ocultamos ambos paneles
        gameOverPanel.SetActive(false);
        levelCompletePanel.SetActive(false); // --- NUEVO ---

        // Restablecemos los valores
        currentPlayerHealth = maxPlayerHealth;
        currentScore = 0;
        if (kiroHealthSlider != null)
        {
            currentKiroHealth = maxKiroHealth;
            kiroHealthSlider.value = maxKiroHealth;
        }
        healthSlider.value = currentPlayerHealth;
        scoreText.text = "Monedas: 0";
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

    // --- NUEVO: Función para el botón "Siguiente Nivel" ---
    public void OnNextLevelButton()
    {
        Time.timeScale = 1f; // Quita la pausa
        levelCompletePanel.SetActive(false); // Oculta el panel

        // ¡Importante! El GameManager persistente cargará el Nivel_2
        // y su función Start() se ejecutará, re-habilitando el input
        // y reseteando los valores (si así lo decides).
        // Por ahora, solo cargamos la escena.
        SceneManager.LoadScene(nextLevelName);
    }

    // Esta función la usan AMBOS paneles
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        Destroy(hudCanvas);
        Destroy(gameObject);
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // --- El resto de tus funciones ---
    // (TakePlayerDamage, AddScore, PlayerDie, KiroDies, HealKiro...)
    // No necesitan cambios.
    
    public void TakePlayerDamage(int damage)
    {
        currentPlayerHealth -= damage;
        if (currentPlayerHealth < 0) { currentPlayerHealth = 0; }
        healthSlider.value = currentPlayerHealth;
        if (currentPlayerHealth <= 0) { PlayerDie(); }
    }
    public void AddScore(int points)
    {
        currentScore += points;
        scoreText.text = "Monedas: " + currentScore;
    }
    void PlayerDie()
    {
        if (playerInput != null) { playerInput.DeactivateInput(); } // Desactiva el input al morir
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }
    void KiroDies()
    {
        Debug.Log("¡Kiro ha muerto! Game Over.");
        if (playerInput != null) { playerInput.DeactivateInput(); } // Desactiva el input
        gameOverPanel.SetActive(true); 
        Time.timeScale = 0f;
    }
    public void HealKiro(float healAmount)
    {
        currentKiroHealth += healAmount;
        if (currentKiroHealth > maxKiroHealth) { currentKiroHealth = maxKiroHealth; }
        kiroHealthSlider.value = currentKiroHealth;
    }
}