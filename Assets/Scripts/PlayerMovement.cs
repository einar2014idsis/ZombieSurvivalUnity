using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Velocidades")]
    public float walkSpeed = 2.8f;
    public float runSpeed = 4.4f;

    [Header("Giro")]
    public float turnSpeedDeg = 140f;

    [Header("Salto / Gravedad")]
    public float jumpForce = 5f;
    public float gravity = -9.81f;

    [Header("Suavizado")]
    public float accel = 12f;
    public float decel = 16f;

    [Header("Entrada")]
    public float deadZone = 0.15f; // filtra ruido

    CharacterController controller;
    Animator animator;

    float yVel;
    float planarSpeed;
    float targetSpeed;

    // Estado de “parado” latcheado por S
    bool isStopping = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 🚫 SOLUCIÓN AL ERROR:
        // Si el CharacterController está apagado (GameTimer lo desactiva al inicio),
        // no intentamos mover al jugador.
        if (controller == null || !controller.enabled)
            return;

        // --- Entradas ---
        bool forwardKey = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool leftKey = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        bool rightKey = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        bool sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // --- Toggle de Stop con S (una sola pulsación) ---
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            isStopping = true;

        // Si el jugador intenta moverse (W/A/D, flechas) o salta, salimos de Stop
        bool wantsMove = forwardKey || leftKey || rightKey;
        if (wantsMove || Input.GetButtonDown("Jump"))
            isStopping = false;

        // Avisar al Animator
        if (animator) animator.SetBool("IsStopping", isStopping);

        // --- Giro (sin strafe) ---
        float turnInput = 0f;
        if (leftKey) turnInput -= 1f;
        if (rightKey) turnInput += 1f;
        if (Mathf.Abs(turnInput) < deadZone) turnInput = 0f;

        if (!isStopping && turnInput != 0f)
        {
            float turn = turnInput * turnSpeedDeg * Time.deltaTime;
            transform.Rotate(0f, turn, 0f, Space.Self);
        }

        // --- Avance recto (W = avanza; en Stop = 0) ---
        float desire = (!isStopping && forwardKey) ? 1f : 0f;

        float maxSpeed = sprint ? runSpeed : walkSpeed;
        targetSpeed = desire * maxSpeed;

        float lerpRate = (targetSpeed > planarSpeed) ? accel : decel;
        planarSpeed = Mathf.Lerp(planarSpeed, targetSpeed, lerpRate * Time.deltaTime);

        Vector3 planar = transform.forward * planarSpeed;

        // --- Salto + gravedad ---
        bool grounded = controller.isGrounded;
        if (grounded && yVel < 0f) yVel = -2f;

        if (!isStopping && grounded && Input.GetButtonDown("Jump"))
        {
            yVel = Mathf.Sqrt(jumpForce * -2f * gravity); // gravity negativa
            if (animator) animator.SetTrigger("Jump");
        }

        yVel += gravity * Time.deltaTime;

        Vector3 total = planar;
        total.y = yVel;
        controller.Move(total * Time.deltaTime);

        // --- Animator params de locomoción ---
        if (animator)
        {
            float verticalParam = (maxSpeed > 0.001f) ? (planarSpeed / maxSpeed) : 0f;
            verticalParam = Mathf.Clamp01(verticalParam);

            animator.SetFloat("Horizontal", 0f);                  // sin lateral
            animator.SetFloat("Vertical", verticalParam);        // 0..1
            animator.SetFloat("Speed", planarSpeed / runSpeed);
            animator.SetBool("IsGrounded", grounded);
            animator.SetBool("IsSprinting", sprint);
        }
    }
}
