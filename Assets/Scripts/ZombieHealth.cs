using UnityEngine;
using UnityEngine.AI;

public class ZombieHealth : MonoBehaviour
{
    [Header("Vida del Zombie")]
    public float maxHealth = 50f;
    private float currentHealth;

    private Animator animator;
    private NavMeshAgent agent;
    private bool isDead = false;

    private GameTimer gameTimer;

    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        // Buscar el GameTimer una sola vez
        gameTimer = FindObjectOfType<GameTimer>();
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Daño cuando el arma entra en el trigger
    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        WeaponDamageComponent weapon = other.GetComponent<WeaponDamageComponent>();
        if (weapon != null)
        {
            TakeDamage(weapon.damage);
            Debug.Log("Zombie recibió daño de arma: " + weapon.damage);
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (animator != null)
            animator.SetBool("IsDead", true);

        if (agent != null)
            agent.isStopped = true;

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        // Avisar al GameTimer que este zombi murió
        if (gameTimer != null)
        {
            gameTimer.RegisterKill(1);
        }

        // Desaparece después de 5 segundos
        Destroy(gameObject, 5f);
    }
}
