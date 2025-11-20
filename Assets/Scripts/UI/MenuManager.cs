using UnityEngine;
using UnityEngine.SceneManagement; 

public class MenuManager : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    public string tutorialSceneName = "Tutorial"; // La escena de historia
    public string firstLevelSceneName = "Nivel_1";

    // --- NUEVA PARTIDA ---
    public void OnNewGameButton()
    {
        // Carga la primera escena del juego
        PlayerPrefs.DeleteAll(); 
        
        SceneManager.LoadScene(tutorialSceneName);    }

    // --- CONTINUAR PARTIDA ---
    public void OnContinueButton()
    {
        // Verificamos si hay un nivel guardado
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            string levelToLoad = PlayerPrefs.GetString("SavedLevel");
            Debug.Log("Cargando nivel guardado: " + levelToLoad);
            SceneManager.LoadScene(levelToLoad);
        }
        else
        {
            Debug.Log("No hay partida guardada. Iniciando nueva.");
            // Si no hay nada guardado, inicia desde el principio (o tutorial)
            SceneManager.LoadScene(tutorialSceneName);
        }
    }

    // --- SALIR DEL JUEGO ---
    public void OnQuitButton()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit(); // Esta función solo funciona en el juego compilado, no en el editor.
    }
}