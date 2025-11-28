using UnityEngine;

public class ZombieFixHeight : MonoBehaviour
{
    private float initialY;

    void Start()
    {
        // Guardamos la altura inicial del zombie en el mundo
        initialY = transform.position.y;
    }

    void LateUpdate()
    {
        // Bloqueamos la Y para que nunca se hunda ni suba
        Vector3 pos = transform.position;
        pos.y = initialY;
        transform.position = pos;
    }
}
