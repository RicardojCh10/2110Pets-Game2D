using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // --- Singleton Pattern ---
    public static GameManager Instance;

    [Header("Referencias de la UI")]
    public GameObject hudCanvas; // El Canvas completo
    public Slider healthSlider;
    public TextMeshProUGUI scoreText;
    public GameObject gameOverPanel; // El panel que está desactivado

    [Header("Variables del Jugador")]
    public int maxPlayerHealth = 100;
    private int currentPlayerHealth;
    private int currentScore = 0;

    private void Awake()
    {
        // Configuración del Singleton
        if (Instance == null)
        {
            Instance = this;
            // Hacemos que el GameManager sea persistente
            DontDestroyOnLoad(gameObject); 
            // Hacemos que TODO el Canvas de UI sea persistente
            DontDestroyOnLoad(hudCanvas); 
        }
        else
        {
            // Si ya existe uno (de una escena anterior), destruye este nuevo
            Destroy(gameObject);
            Destroy(hudCanvas); // También destruye el canvas duplicado
        }
    }

    void Start()
    {
        // Inicializamos los valores al empezar el juego
        currentPlayerHealth = maxPlayerHealth;
        
        healthSlider.maxValue = maxPlayerHealth;
        healthSlider.value = currentPlayerHealth;
        scoreText.text = "Monedas: 0";

        // Nos aseguramos de que el panel de Game Over esté apagado
        gameOverPanel.SetActive(false); 
    }

    // --- Funciones Públicas ---

    public void TakePlayerDamage(int damage)
    {
        currentPlayerHealth -= damage;
        if (currentPlayerHealth < 0)
        {
            currentPlayerHealth = 0;
        }

        healthSlider.value = currentPlayerHealth; // Actualiza la barra de vida

        if (currentPlayerHealth <= 0)
        {
            PlayerDie();
        }
    }

    public void AddScore(int points)
    {
        currentScore += points;
        scoreText.text = "Monedas: " + currentScore; // Actualiza el texto
    }

    void PlayerDie()
    {
        gameOverPanel.SetActive(true); // Muestra la pantalla de Game Over
        Time.timeScale = 0f; // Pausa el juego
    }

    // Esta función la llamará el botón "RetryButton"
    public void OnRetryButton()
    {
        // 1. Reanuda el juego
        Time.timeScale = 1f; 
        
        // 2. Apaga el panel de Game Over
        gameOverPanel.SetActive(false); 

        // 3. Restablece todos los valores del juego a su estado inicial
        currentPlayerHealth = maxPlayerHealth;
        currentScore = 0;
        
        // Asumiendo que Kiro también debe revivir
        // if (kiroHealthSlider != null) // (Comprobación por si acaso)
        // {
        //     currentKiroHealth = maxKiroHealth;
        //     kiroHealthSlider.value = maxKiroHealth;
        // }

        // 4. Actualiza la UI para que muestre los valores nuevos
        healthSlider.value = currentPlayerHealth;
        scoreText.text = "Monedas: 0";
        
        // 5. Vuelve a cargar la escena
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

    public void QuitToMainMenu()
{
    // 1. Reanudamos el juego
    Time.timeScale = 1f;

    // 2. Destruimos los objetos persistentes para que no viajen al menú
    Destroy(hudCanvas);       // Destruye el Canvas de la UI
    Destroy(gameObject);    // Destruye este objeto (_GameManager)

    // 3. Cargamos la escena del Menú Principal
    SceneManager.LoadScene("MainMenu");
}
}