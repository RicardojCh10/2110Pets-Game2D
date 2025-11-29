using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Referencias de la UI")]
    public GameObject hudCanvas;
    public Slider healthSlider; 
    public GameObject scoreContainer; 
    public TextMeshProUGUI scoreText;
    public GameObject gameOverPanel;
    public GameObject levelCompletePanel;
    public TextMeshProUGUI levelCompleteText; 

    [Header("Referencias del Jugador")]
    public PlayerInput playerInput;

    [Header("Variables del Jugador")]
    public int maxPlayerHealth = 100;
    private int currentPlayerHealth;
    private int currentScore = 0; 

    [Header("Configuración de Nivel")]
    public string[] gameLevelSceneNames;
    public string mainMenuSceneName = "MainMenu";
    private int currentLevelIndex = 0;

    [Header("Audio del Jugador")]
    public AudioClip playerDeathSound; 
    private AudioSource aidenAudioSource; 
    
    [Header("Audio de Música de Nivel")]
    public AudioSource musicSource;         
    public AudioClip level1Music;           
    public AudioClip level2Music;         
    public AudioClip level3Music;      

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
            DontDestroyOnLoad(hudCanvas); 
            musicSource = GetComponent<AudioSource>();
            if (musicSource == null)
            {
                Debug.LogError("GameManager requiere un AudioSource para la música de fondo.");
            }
        }
        else
        {
            Destroy(gameObject);
            Destroy(hudCanvas);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == mainMenuSceneName) return; 

        PlayerPrefs.SetString("SavedLevel", scene.name);
        PlayerPrefs.Save();
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

            ManageBackgroundMusic(scene.name);

        }

        if (scene.name != mainMenuSceneName)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                aidenAudioSource = playerObject.GetComponent<AudioSource>();
            }
        }
    }

        void ManageBackgroundMusic(string sceneName)
    {
        AudioClip clipToPlay = null;

        if (sceneName == gameLevelSceneNames[0])
        {
            clipToPlay = level1Music;
        }
        else if (sceneName == gameLevelSceneNames[1])
        {
            clipToPlay = level2Music;
        }
        else if (sceneName == gameLevelSceneNames[2])
        {
            clipToPlay = level3Music;
        }

        if (musicSource != null && clipToPlay != null && musicSource.clip != clipToPlay)
        {
            musicSource.clip = clipToPlay;
            musicSource.loop = true; 
            musicSource.Play();
        }
    }

    void SetupNewLevel()
    {
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

        Time.timeScale = 1f; 

        currentPlayerHealth = maxPlayerHealth;
        if(healthSlider != null) 
        {
            healthSlider.maxValue = maxPlayerHealth; 
            healthSlider.value = maxPlayerHealth;
        }

        if(scoreText != null) scoreText.text = "Monedas: " + currentScore;

        if(gameOverPanel != null) gameOverPanel.SetActive(false); 
        if(levelCompletePanel != null) levelCompletePanel.SetActive(false);
    }

    

    public void CompleteLevel()
    {
        if (playerInput != null) playerInput.DeactivateInput();
        Time.timeScale = 0f;

        int levelNumber = currentLevelIndex + 1;

        if (levelCompleteText != null)
        {
            if (currentLevelIndex == gameLevelSceneNames.Length - 1)
            {
                levelCompleteText.text = "¡VICTORIA! ¡Has completado el juego! ";
            }
            else
            {
                levelCompleteText.text = $"¡Felicidades! Has completado el Nivel {levelNumber} ";
            }
        }

        if(levelCompletePanel != null) levelCompletePanel.SetActive(true);
    }


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
            Debug.LogWarning("GameManager: No se encontró el HealthSlider para actualizar la vida.");
        }
        
        if (currentPlayerHealth <= 0) { PlayerDie(); }
    }



    public void HealPlayer(int amount)
    {
        currentPlayerHealth += amount;
        
        if (currentPlayerHealth > maxPlayerHealth)
        {
            currentPlayerHealth = maxPlayerHealth;
        }

        if(healthSlider != null) 
        {
            healthSlider.value = currentPlayerHealth;
        }
    }

    public void AddScore(int points)
    {
        currentScore += points;
        if(scoreText != null) scoreText.text = "Monedas: " + currentScore;
    }

    void PlayerDie()
    {
        if (aidenAudioSource != null && playerDeathSound != null)
        {
            aidenAudioSource.PlayOneShot(playerDeathSound);
        }

        if (playerInput != null) { playerInput.DeactivateInput(); }
        if(gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }
}