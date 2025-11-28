using UnityEngine;

public class ZombieLockY : MonoBehaviour
{
    private float _initialLocalY;

    void Start()
    {
        // Guardamos la altura LOCAL inicial del modelo (hijo)
        _initialLocalY = transform.localPosition.y;
    }

    void LateUpdate()
    {
        // Bloqueamos la Y local para que la animación no lo hunda ni lo suba
        Vector3 localPos = transform.localPosition;
        localPos.y = _initialLocalY;
        transform.localPosition = localPos;
    }
}
