using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Configuración de vida")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [Header("UI (opcional)")]
    public Slider healthBar;

    [Header("Eventos")]
    public UnityEvent onDeath;                 // "On Death"
    public UnityEvent<float> onHealthChanged;  // "On Health Changed (Single)"

    [Header("Opciones")]
    [Tooltip("Si es true, este objeto se destruirá al morir. Para el Player ponlo en false.")]
    public bool destroyOnDeath = true;

    bool isDead = false;

    void Start()
    {
        // Vida inicial
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateUI();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;
        if (amount <= 0f) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        float normalized = maxHealth > 0f ? currentHealth / maxHealth : 0f;

        // Aviso por evento
        if (onHealthChanged != null)
            onHealthChanged.Invoke(normalized);

        UpdateUI();

        if (currentHealth <= 0f)
            Die();
    }

    public void Heal(float amount)
    {
        if (isDead) return;
        if (amount <= 0f) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        float normalized = maxHealth > 0f ? currentHealth / maxHealth : 0f;

        if (onHealthChanged != null)
            onHealthChanged.Invoke(normalized);

        UpdateUI();
    }

    void UpdateUI()
    {
        if (healthBar != null)
        {
            float normalized = maxHealth > 0f ? currentHealth / maxHealth : 0f;
            healthBar.minValue = 0f;
            healthBar.maxValue = 1f;
            healthBar.value = normalized;
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{name} muerto");

        // Llamamos a los eventos configurados en el inspector (GameTimer.PlayerDied para el Player)
        if (onDeath != null)
            onDeath.Invoke();

        // IMPORTANTE:
        // NO destruir este GameObject para el Player.
        // (Si quieres que los zombis sí se destruyan, usa otro script para ellos,
        // o crea un bool destroyOnDeath separado.)
    }
}