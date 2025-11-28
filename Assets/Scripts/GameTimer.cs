using System.Collections;
using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject player;                 // Player (raíz)
    public CharacterController playerController;
    public Animator playerAnimator;
    public ZombieSpawner zombieSpawner;       // Spawner de zombis

    [Header("UI")]
    public TMP_Text countdownText;            // "Empieza en: 5"
    public TMP_Text matchTimerText;           // "Tiempo: 05:00"
    public TMP_Text messageText;              // Mensajes de ronda / victoria / derrota
    public TMP_Text killsText;                // "Zombis: 0/30"

    [Header("Scripts que se habilitan al empezar")]
    public MonoBehaviour[] scriptsToEnable;   // Por ejemplo: PlayerMovement

    [Header("Tiempo por ronda")]
    public int roundDuration = 300;           // segundos (5 minutos)
    public float startDelay = 5f;             // cuenta atrás inicial

    [Header("Kills requeridas por ronda")]
    public int killsRound1 = 30;
    public int killsRound2 = 25;
    public int killsRound3 = 1;

    [Header("Victoria")]
    public float victoryDelay = 2f;           // segundos que esperamos tras matar al jefe

    int currentRound = 1;
    int currentKills = 0;
    float timeRemaining;
    bool running = false;
    bool finished = false;

    void Awake()
    {
        if (playerController == null && player != null)
            playerController = player.GetComponent<CharacterController>();

        if (zombieSpawner == null)
        {
            zombieSpawner = FindObjectOfType<ZombieSpawner>();
            if (zombieSpawner == null)
            {
                Debug.LogError("[GameTimer] No se encontró ningún ZombieSpawner en la escena.");
            }
        }
    }

    void Start()
    {
        // Solo apagamos scripts de control al inicio
        foreach (var s in scriptsToEnable)
        {
            if (s) s.enabled = false;
        }

        currentRound = 1;
        currentKills = 0;
        timeRemaining = roundDuration;
        running = false;
        finished = false;

        UpdateKillsText();
        UpdateTimerText();

        StartCoroutine(CountdownAndStartRound());
    }

    IEnumerator CountdownAndStartRound()
    {
        float t = startDelay;
        while (t > 0f)
        {
            if (countdownText)
            {
                int sec = Mathf.CeilToInt(t);
                countdownText.text = $"Empieza en: {sec}";
            }
            t -= Time.deltaTime;
            yield return null;
        }

        if (countdownText) countdownText.text = string.Empty;

        BeginRound(1);
    }

    void Update()
    {
        if (!running || finished) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            HandleTimeOut();
        }

        UpdateTimerText();
    }

    void BeginRound(int round)
    {
        if (finished) return;

        currentRound = Mathf.Clamp(round, 1, 3);
        currentKills = 0;
        timeRemaining = roundDuration;
        running = true;

        int totalToSpawn = GetKillsTargetForRound(currentRound);
        Debug.Log($"[GameTimer] Empezando RONDA {currentRound} con {totalToSpawn} zombis");

        if (messageText)
        {
            if (currentRound == 1)
                messageText.text = string.Empty;
            else
                messageText.text = $"Ronda {currentRound}";
        }

        UpdateKillsText();
        UpdateTimerText();

        foreach (var s in scriptsToEnable)
        {
            if (s) s.enabled = true;
        }

        if (zombieSpawner != null)
        {
            zombieSpawner.StartRound(currentRound, totalToSpawn);
        }
    }

    void HandleTimeOut()
    {
        running = false;

        if (currentRound < 3)
        {
            BeginRound(currentRound + 1);
        }
        else
        {
            EndMatch(false, "Se acabó el tiempo. Has perdido la batalla.");
        }
    }

    int GetKillsTargetForRound(int round)
    {
        switch (round)
        {
            case 1: return killsRound1;
            case 2: return killsRound2;
            case 3: return killsRound3;
            default: return killsRound1;
        }
    }

    void UpdateKillsText()
    {
        if (!killsText) return;

        int target = GetKillsTargetForRound(currentRound);
        killsText.text = $"Zombis: {currentKills}/{target}";
    }

    void UpdateTimerText()
    {
        if (!matchTimerText) return;

        int total = Mathf.RoundToInt(timeRemaining);
        int m = total / 60;
        int s = total % 60;
        matchTimerText.text = $"Tiempo: {m:00}:{s:00}";
    }

    // Llamado por ZombieHealth cuando un zombi muere
    public void RegisterKill(int amount)
    {
        if (finished) return;

        currentKills += amount;
        if (currentKills < 0) currentKills = 0;
        UpdateKillsText();

        int target = GetKillsTargetForRound(currentRound);
        if (currentKills >= target)
        {
            running = false;

            if (currentRound >= 3)
            {
                StartCoroutine(VictoryAfterDelay());
            }
            else
            {
                BeginRound(currentRound + 1);
            }
        }
    }

    IEnumerator VictoryAfterDelay()
    {
        yield return new WaitForSeconds(victoryDelay);

        if (!finished)
        {
            EndMatch(true, "¡Victoria! Zona despejada.");
        }
    }

    public void EndMatch(bool win, string msg)
    {
        finished = true;
        running = false;

        foreach (var s in scriptsToEnable)
        {
            if (s) s.enabled = false;
        }

        if (playerAnimator)
        {
            playerAnimator.SetFloat("Speed", 0f);
        }

        if (messageText) messageText.text = msg;
    }

    public void PlayerDied()
    {
        if (finished) return;
        EndMatch(false, "Has muerto");
    }
}
