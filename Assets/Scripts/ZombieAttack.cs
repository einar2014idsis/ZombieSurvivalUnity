using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class ZombieAttack : MonoBehaviour
{
    [Header("Daño por segundo mientras toca al jugador")]
    public float damagePerSecond = 50f;   // súbelo para probar mejor

    // Un bucle por cada Health al que estamos tocando
    private readonly Dictionary<Health, Coroutine> _loops = new();

    private void Reset()
    {
        // Configuración básica del trigger
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;

        var rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[ZombieAttack] OnTriggerEnter con: {other.name}", this);

        // Buscamos Health en el objeto o en sus padres
        Health health = other.GetComponent<Health>() ?? other.GetComponentInParent<Health>();

        if (health == null)
        {
            Debug.Log($"[ZombieAttack] {other.name} NO tiene Health, se ignora.", this);
            return;
        }

        if (_loops.ContainsKey(health)) return;

        Debug.Log($"[ZombieAttack] Empezando a hacer daño a: {health.name}", this);
        Coroutine co = StartCoroutine(DamageLoop(health));
        _loops[health] = co;
    }

    private void OnTriggerExit(Collider other)
    {
        Health health = other.GetComponent<Health>() ?? other.GetComponentInParent<Health>();
        if (health == null) return;

        if (_loops.TryGetValue(health, out var co))
        {
            Debug.Log($"[ZombieAttack] Dejó de tocar a: {health.name}", this);
            StopCoroutine(co);
            _loops.Remove(health);
        }
    }

    private IEnumerator DamageLoop(Health target)
    {
        while (target != null && target.currentHealth > 0f)
        {
            target.TakeDamage(damagePerSecond * Time.deltaTime);
            yield return null;
        }

        if (target != null && _loops.ContainsKey(target))
        {
            _loops.Remove(target);
        }
    }
}
