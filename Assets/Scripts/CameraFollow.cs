using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow")]
    public Transform target;                      // arrastra aquí "CameraTarget" (hijo del Player)
    public Vector3 offset = new Vector3(0, 2.5f, -5f);

    [Header("Mouse")]
    public float rotateSpeed = 120f;              // sensibilidad
    public float minPitch = -25f;
    public float maxPitch = 65f;
    public bool lockCursor = true;                // bloquea/oculta el cursor al entrar en Play

    float yaw, pitch;

    void OnEnable()
    {
        // bloquea y oculta cursor si se desea
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void OnDisable()
    {
        // al salir de Play, deja el cursor normal
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Start()
    {
        var e = transform.eulerAngles;
        yaw = e.y;
        pitch = e.x;
    }

    void Update()
    {
        // desbloquear temporalmente con ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // volver a bloquear con clic izquierdo
        if (Input.GetMouseButtonDown(0) && lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // leer movimiento del mouse
        yaw += Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // colocar cámara (sin zoom)
        var rot = Quaternion.Euler(pitch, yaw, 0f);
        transform.position = target.position + rot * offset;
        transform.rotation = rot;
    }
}
