using UnityEngine;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class ZombieStayOnGround : MonoBehaviour
{
    private float initialY;

    void Start()
    {
        // Guardamos la altura inicial del zombie en el mundo
        initialY = transform.position.y;
    }

    void LateUpdate()
    {
        // Forzamos que siempre tenga la misma Y
        Vector3 pos = transform.position;
        pos.y = initialY;
        transform.position = pos;
    }
}
