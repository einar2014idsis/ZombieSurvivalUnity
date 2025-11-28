using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    public Animator animator;

    [Header("Double Click Settings")]
    public float doubleClickThreshold = 0.25f;

    [Header("Animator Triggers")]
    public string lightAttackParam = "LightAttack";         // un clic
    public string transformAttackParam = "TransformAttack"; // doble clic
    public string rightAttackParam = "RightAttack";         // clic derecho

    private float lastLeftClickTime = -1f;

    void Reset()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!animator) return;

        // 👊 Clic izquierdo - simple o doble
        if (Input.GetMouseButtonDown(0))
        {
            float now = Time.time;

            if (now - lastLeftClickTime <= doubleClickThreshold)
            {
                // Doble clic → segunda animación
                animator.ResetTrigger(lightAttackParam);
                animator.SetTrigger(transformAttackParam);
                lastLeftClickTime = -1f;
            }
            else
            {
                // Primer clic → primera animación
                animator.SetTrigger(lightAttackParam);
                lastLeftClickTime = now;
            }
        }

        // ⚔️ Clic derecho - tercera animación
        if (Input.GetMouseButtonDown(1))
        {
            animator.SetTrigger(rightAttackParam);
        }
    }
}
