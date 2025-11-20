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
    public TextMeshProUGUI scoreText;
    public GameObject gameOverPanel;
    public GameObject levelCompletePanel;
    public TextMeshProUGUI levelCompleteText; //  Referencia al texto

    [Header("Referencias del Jugador")]
    public PlayerInput playerInput; // Lo encontraremos automáticamente

    [Header("Variables del Jugador")]
    public int maxPlayerHealth = 100;
    private int currentPlayerHealth;
    private int currentScore = 0; // El puntaje se mantiene entre niveles

    // --- MODIFICADO: Lista de niveles ---
    [Header("Configuración de Nivel")]
    public string[] gameLevelSceneNames; // La lista de tus niveles
    public string mainMenuSceneName = "MainMenu";
    private int currentLevelIndex = 0;


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
        if (scene.name == mainMenuSceneName) return; // Si es el menú, no hagas nada

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
    }

    // Lógica de reinicio para CADA nivel
    void SetupNewLevel()
    {
        // 1. Encontrar al nuevo jugador (¡NECESITA EL TAG "Player"!)
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
            healthSlider.maxValue = maxPlayerHealth; // <-- Asegúrate de poner el MaxValue
            healthSlider.value = maxPlayerHealth;
        }

        // 4. Actualizar puntaje (NO lo reiniciamos, se mantiene)
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
    
    // --- El resto de tus funciones ---
    
    public void TakePlayerDamage(int damage)
    {
        currentPlayerHealth -= damage;
        if (currentPlayerHealth < 0) { currentPlayerHealth = 0; }
        
        // Esta es la línea importante para la barra de vida de Aiden
        if(healthSlider != null) 
        {
            healthSlider.value = currentPlayerHealth;
        }
        
        if (currentPlayerHealth <= 0) { PlayerDie(); }
    }

    public void AddScore(int points)
    {
        currentScore += points;
        if(scoreText != null) scoreText.text = "Monedas: " + currentScore;
    }

    void PlayerDie()
    {
        if (playerInput != null) { playerInput.DeactivateInput(); }
        if(gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }
}