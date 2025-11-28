using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class ZombieAttackAnimator : MonoBehaviour
{
    public Transform player;
    public float attackRange = 1.8f;        // distancia para atacar
    public float timeBetweenAttacks = 1.2f; // segundos entre ataques

    private Animator animator;
    private NavMeshAgent agent;
    private float attackTimer;

    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        // Parametro Speed para Idle/Walk
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);

        // Distancia plana al jugador
        Vector3 myPos = transform.position;
        Vector3 targetPos = player.position;
        myPos.y = targetPos.y = 0f;
        float dist = Vector3.Distance(myPos, targetPos);

        attackTimer -= Time.deltaTime;

        // Mientras esté cerca y haya pasado el cooldown, atacar
        if (dist <= attackRange && attackTimer <= 0f)
        {
            bool useBite = Random.value < 0.5f;

            if (useBite)
                animator.SetTrigger("DoAttackBite");
            else
                animator.SetTrigger("DoAttackHand");

            attackTimer = timeBetweenAttacks;
        }
    }
}
