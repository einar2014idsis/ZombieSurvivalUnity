using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Nombre de la escena del juego")]
    [SerializeField] private string gameSceneName = "trabajoUNI";

    [Header("Referencia al Loading Screen (opcional)")]
    [SerializeField] private LoadingScreen loader;

    public void PlayGame()
    {
        // Debug opcional:
        // Debug.Log("BOTON PLAY ✅");

        Time.timeScale = 1f;
        PlayerPrefs.SetInt("StartImmediate", 1);
        PlayerPrefs.Save();

        if (loader != null) loader.LoadScene(gameSceneName);
        else SceneManager.LoadScene(gameSceneName);
    }
}
