using UnityEngine;
using UnityEngine.SceneManagement; // ¡¡MUY IMPORTANTE!!

public class MenuManager : MonoBehaviour
{
    // Esta variable la pones en el Inspector.
    // Escribe el nombre exacto de tu primera escena de juego (ej. "Nivel_1")
    public string firstLevelSceneName = "Nivel_1";

    // Esta función la llamará el botón "Nueva Partida"
    public void OnNewGameButton()
    {
        // Carga la primera escena del juego
        SceneManager.LoadScene(firstLevelSceneName);
    }

    // Esta función la llamará el botón "Continuar"
    public void OnContinueButton()
    {
        // (Por ahora, la dejaremos simple. Más adelante, aquí irá la lógica
        // para cargar datos guardados)
        Debug.Log("Cargando partida guardada... (lógica no implementada)");
        // Si tienes guardado el nivel, lo cargarías aquí.
        // Por ahora, solo iniciaremos el primer nivel.
        SceneManager.LoadScene(firstLevelSceneName);
    }

    // Esta función la llamará el botón "Salir"
    public void OnQuitButton()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit(); // Esta función solo funciona en el juego compilado, no en el editor.
    }
}