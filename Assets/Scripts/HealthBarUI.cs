using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("Referencias")]
    public Health playerHealth;     // arrastra el Health del Player
    public Slider healthSlider;     // arrastra HP_Slider

    void Awake()
    {
        if (!playerHealth)
            Debug.LogWarning($"HealthBarUI ({gameObject.name}): Falta Player Health.");
        if (!healthSlider)
            Debug.LogWarning($"HealthBarUI ({gameObject.name}): Falta Health Slider.");
    }

    void OnEnable()
    {
        if (playerHealth != null)
            playerHealth.onHealthChanged.AddListener(UpdateBar);
    }

    void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.onHealthChanged.RemoveListener(UpdateBar);
    }

    void Start()
    {
        if (healthSlider != null)
            healthSlider.value = 1f;
    }

    // Este método lo llama el evento onHealthChanged del Health
    void UpdateBar(float normalized)
    {
        if (healthSlider != null)
            healthSlider.value = normalized;
    }
}
