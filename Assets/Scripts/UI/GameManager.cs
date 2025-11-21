using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    // --- Singleton Pattern ---
    public static GameManager Instance;

    [Header("Referencias de la UI")]
    public GameObject hudCanvas;
    public Slider healthSlider; // <-- La barra de vida de Aiden
    public GameObject scoreContainer; 
    public TextMeshProUGUI scoreText;
    public GameObject gameOverPanel;
    public GameObject levelCompletePanel;
    public TextMeshProUGUI levelCompleteText; //  Referencia al texto

    [Header("Referencias del Jugador")]
    public PlayerInput playerInput;

    [Header("Variables del Jugador")]
    public int maxPlayerHealth = 100;
    private int currentPlayerHealth;
    private int currentScore = 0; // El puntaje se mantiene entre niveles

    // --- Lista de niveles ---
    [Header("Configuración de Nivel")]
    public string[] gameLevelSceneNames;
    public string mainMenuSceneName = "MainMenu";
    private int currentLevelIndex = 0;

    // Variables de Audio
    [Header("Audio del Jugador")]
    public AudioClip playerDeathSound; 
    private AudioSource aidenAudioSource; 


    private void Awake()
    {
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

    // --- Detectar cargas de escenas ---
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // --- Se llama CADA VEZ que se carga una escena ---
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == mainMenuSceneName) return; 

        PlayerPrefs.SetString("SavedLevel", scene.name);
        PlayerPrefs.Save();

        // Sincroniza el índice del nivel actual
        bool foundLevel = false;
        for (int i = 0; i < gameLevelSceneNames.Length; i++)
        {
            if (gameLevelSceneNames[i] == scene.name)
            {
                currentLevelIndex = i;
                foundLevel = true;
                break;
            }
        }

        if (foundLevel)
        {
            SetupNewLevel();
        }

        // Configurar la fuente de audio de Aiden
        if (scene.name != mainMenuSceneName)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                aidenAudioSource = playerObject.GetComponent<AudioSource>();
            }
        }
    }

    // Lógica de reinicio para CADA nivel
    void SetupNewLevel()
    {
        // 1. Encontrar al nuevo jugador
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerInput = playerObject.GetComponent<PlayerInput>();
            playerInput.ActivateInput();
        }
        else
        {
            Debug.LogError("¡No se encontró al jugador! Asegúrate de que Aiden tenga el Tag 'Player'.");
        }

        // 2. Reiniciar el tiempo
        Time.timeScale = 1f; 

        // 3. Reiniciar estadísticas del jugador
        currentPlayerHealth = maxPlayerHealth;
        if(healthSlider != null) 
        {
            healthSlider.maxValue = maxPlayerHealth; 
            healthSlider.value = maxPlayerHealth;
        }

        // 4. Actualizar puntaje en la UI
        if(scoreText != null) scoreText.text = "Monedas: " + currentScore;

        // 5. Ocultar paneles
        if(gameOverPanel != null) gameOverPanel.SetActive(false); 
        if(levelCompletePanel != null) levelCompletePanel.SetActive(false);
    }

    

    public void CompleteLevel()
    {
        // 1. Desactivar control
        if (playerInput != null) playerInput.DeactivateInput();
        Time.timeScale = 0f; // Pausa el juego

        // 2. Calcular el número del nivel (basado en el índice)
        int levelNumber = currentLevelIndex + 1;

        // 3. Escribir el mensaje correcto
        if (levelCompleteText != null)
        {
            // Comprueba si es el último nivel (el jefe final)
            if (currentLevelIndex == gameLevelSceneNames.Length - 1)
            {
                levelCompleteText.text = "¡VICTORIA! ¡Has completado el juego! ";
            }
            else
            {
                levelCompleteText.text = $"¡Felicidades! Has completado el Nivel {levelNumber} ";
            }
        }

        // 4. Mostrar el panel
        if(levelCompletePanel != null) levelCompletePanel.SetActive(true);
    }

    // --- Funciones de Botones ---

    public void OnRetryButton()
    {
        Time.timeScale = 1f;
        currentScore = 0; 
        SceneManager.LoadScene(gameLevelSceneNames[currentLevelIndex]); 
    }

    public void OnNextLevelButton()
    {
        Time.timeScale = 1f;
        int nextIndex = currentLevelIndex + 1;

        if (nextIndex < gameLevelSceneNames.Length)
        {
            SceneManager.LoadScene(gameLevelSceneNames[nextIndex]);
        }
        else
        {
            Debug.Log("¡HAS GANADO EL JUEGO! Regresando al menú...");
            QuitToMainMenu();
        }
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        Destroy(hudCanvas);
        Destroy(gameObject);
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void TakePlayerDamage(int damage)
    {
        currentPlayerHealth -= damage;
        if (currentPlayerHealth < 0) { currentPlayerHealth = 0; }
        
        if(healthSlider != null) 
        {
            healthSlider.value = currentPlayerHealth;
        }
        else
        {
            // Opcional: Intentar reconectar si se perdió
            Debug.LogWarning("GameManager: No se encontró el HealthSlider para actualizar la vida.");
        }
        
        if (currentPlayerHealth <= 0) { PlayerDie(); }
    }



    // --- Curar al Jugador ---
    public void HealPlayer(int amount)
    {
        currentPlayerHealth += amount;
        
        // No dejes que la vida supere el máximo
        if (currentPlayerHealth > maxPlayerHealth)
        {
            currentPlayerHealth = maxPlayerHealth;
        }

        // Actualizar la barra de vida visualmente
        if(healthSlider != null) 
        {
            healthSlider.value = currentPlayerHealth;
        }
    }

    // ---  Añadir Puntaje ---
    public void AddScore(int points)
    {
        currentScore += points;
        if(scoreText != null) scoreText.text = "Monedas: " + currentScore;
    }

    // --- Manejar la muerte del jugador ---
    void PlayerDie()
    {
        // Reproducir sonido de muerte
        if (aidenAudioSource != null && playerDeathSound != null)
        {
            aidenAudioSource.PlayOneShot(playerDeathSound);
        }

        if (playerInput != null) { playerInput.DeactivateInput(); }
        if(gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }
}