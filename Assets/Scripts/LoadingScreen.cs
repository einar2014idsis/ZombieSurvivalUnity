using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup canvasGroup;   // CanvasGroup del overlay
    [SerializeField] private TMP_Text loadingText;      // Texto "Cargando... 0%"
    [SerializeField] private Slider progressBar;        // Barra (opcional)
    [SerializeField] private Image darkBackground;      // Imagen negra semitransparente (opcional)

    [Header("Opciones")]
    [SerializeField] private float fadeDuration = 0.35f;   // Velocidad de aparición/desaparición
    [SerializeField] private bool blockInput = true;    // Bloquear clicks mientras carga

    private bool isLoading = false;

    void Awake()
    {
        // Estado inicial oculto
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = blockInput;
        }
        if (progressBar != null) progressBar.value = 0f;
        if (loadingText != null) loadingText.text = "Cargando... 0%";
        if (darkBackground != null) darkBackground.raycastTarget = blockInput;

        // Arranca desactivado (el LoadScene lo activará)
        gameObject.SetActive(false);
    }

    public void LoadScene(string sceneName)
    {
        if (isLoading) return;

        // ✅ Asegura que el objeto esté activo ANTES de iniciar la corrutina
        if (!gameObject.activeSelf) gameObject.SetActive(true);

        StartCoroutine(LoadRoutine(sceneName));
    }

    private IEnumerator LoadRoutine(string sceneName)
    {
        isLoading = true;

        // Fade IN
        yield return StartCoroutine(FadeCanvas(0f, 1f, fadeDuration));

        // Carga asíncrona
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            UpdateUI(op.progress);
            yield return null;
        }

        // 100% visual
        UpdateUI(1f);
        yield return new WaitForSeconds(0.15f);

        // Cambiamos de escena
        op.allowSceneActivation = true;
    }

    private void UpdateUI(float progress01)
    {
        float pct01 = Mathf.Clamp01(progress01 / 0.9f);
        if (progressBar != null) progressBar.value = pct01;
        if (loadingText != null) loadingText.text = $"Cargando... {pct01 * 100f:0}%";
    }

    private IEnumerator FadeCanvas(float from, float to, float time)
    {
        if (canvasGroup == null || time <= 0f)
        {
            if (canvasGroup != null) canvasGroup.alpha = to;
            yield break;
        }

        float t = 0f;
        canvasGroup.alpha = from;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = blockInput;

        while (t < time)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, t / time);
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}
