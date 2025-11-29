using UnityEngine;
using UnityEngine.SceneManagement; 

public class MenuManager : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    public string tutorialSceneName = "Tutorial"; 
    public string firstLevelSceneName = "Nivel_1";

    [Header("Audio del Menú")]
    public AudioSource audioSource;
    public AudioClip menuMusic;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("MenuManager requiere un AudioSource para la música de fondo.");
        }
    }

    public void OnNewGameButton()
    {
        PlayerPrefs.DeleteAll(); 
        
        SceneManager.LoadScene(tutorialSceneName);    }

    public void OnContinueButton()
    {
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            string levelToLoad = PlayerPrefs.GetString("SavedLevel");
            Debug.Log("Cargando nivel guardado: " + levelToLoad);
            SceneManager.LoadScene(levelToLoad);
        }
        else
        {
            Debug.Log("No hay partida guardada. Iniciando nueva.");
            SceneManager.LoadScene(tutorialSceneName);
        }
    }

    public void OnQuitButton()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit(); 
    }
}