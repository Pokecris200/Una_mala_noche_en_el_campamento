using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonOrbitCamera : MonoBehaviour
{
    [Header("Objetivo")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 targetOffset = new Vector3(0f, 1.5f, 0f);

    [Header("Sensibilidad")]
    [SerializeField] private float sensitivityX = 15f;
    [SerializeField] private float sensitivityY = 15f;
    [SerializeField] private float minYAngle = -30f;
    [SerializeField] private float maxYAngle = 60f;

    private float yaw = 0f;
    private float pitch = 15f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yaw = transform.eulerAngles.y;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Seguir al jugador suavemente o de forma directa
        transform.position = target.position + targetOffset;

        // Leer movimiento del mouse con el nuevo Input System
        if (Mouse.current != null)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            yaw += delta.x * sensitivityX * 0.05f;
            pitch -= delta.y * sensitivityY * 0.05f;
            pitch = Mathf.Clamp(pitch, minYAngle, maxYAngle);
        }

        // Rotar el pivote (la cámara hija orbita automáticamente)
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}