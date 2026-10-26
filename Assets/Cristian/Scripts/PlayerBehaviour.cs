using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float speed = 10f;
    private float sprint = 1f;
    [Header("Stamina")]
    [SerializeField] private float maxStamina = 50f;
    [SerializeField] private float stamina = 50f;
    [SerializeField] private float staminaDrain = 15f;    // Puntos de estamina gastados por segundo
    [SerializeField] private float staminaRecovery = 10f;
    Vector2 horizontalInput;
    [SerializeField] private bool running = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 moveDirection = (transform.right * horizontalInput.x) + (transform.forward * horizontalInput.y);

        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        Vector3 targetVelocity = moveDirection * speed * sprint;
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
    }
    void OnMove(InputValue value)
    {
        horizontalInput = value.Get<Vector2>();
    }

    void OnSprint(InputValue value)
    {
        running = value.isPressed;
        
    }

    private void Update()
    {
        bool isMoving = horizontalInput.sqrMagnitude > 0.01f;

        if (running && stamina > 0f && isMoving)
        {
            sprint = 2f;
            stamina -= staminaDrain * Time.deltaTime;

            if (stamina <= 0f)
            {
                stamina = 0f;
                sprint = 1f;
            }
        }
        else
        {
            sprint = 1f;

            if (stamina < maxStamina)
            {
                stamina += staminaRecovery * Time.deltaTime;
                if (stamina > maxStamina)
                {
                    stamina = maxStamina;
                }
            }
        }
    }

}
