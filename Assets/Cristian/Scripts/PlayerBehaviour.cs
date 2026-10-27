using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float speed = 10f;
    private float sprint = 1f;

    [Header("Sprinting")]
    [SerializeField] private float maxStamina = 50f; //Estamina Maxima
    [SerializeField] private float stamina = 50f; //Estamina actual
    [SerializeField] private float staminaDrain = 15f;    // Puntos de estamina gastados por segundo
    [SerializeField] private float staminaRecovery = 10f;
    [SerializeField] private bool running = false;

    [Header("Attack")]
    [SerializeField] private Transform weapon;
    [SerializeField] private float duration = 0.15f; // Tiempo que tarda en ir (y en volver)
    private bool isAttacking = false;

    Vector2 horizontalInput;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void OnMove(InputValue value)
    {
        horizontalInput = value.Get<Vector2>();
    }

    void OnSprint(InputValue value)
    {
        running = value.isPressed;
    }

    void OnAttack(InputValue value)
    {
        AttackInstructions();
    }

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

    private void Update()
    {
        SprintingInstructions();
    }

    void SprintingInstructions()
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

    void AttackInstructions()
    {
        if (!isAttacking)
        {
            StartCoroutine(SwingWeaponRoutine());
        }
    }

    private IEnumerator SwingWeaponRoutine()
    {
        isAttacking = true;

        Quaternion initialRotation = weapon.localRotation;
        Quaternion targetRotation = initialRotation * Quaternion.Euler(45, 0, 0);

        // 1. Rotar hacia el objetivo
        float elapsed = 0f;
        while (elapsed < duration)
        {
            weapon.localRotation = Quaternion.Slerp(initialRotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        weapon.localRotation = targetRotation;

        // 2. Regresar a la posición original
        elapsed = 0f;
        while (elapsed < duration)
        {
            weapon.localRotation = Quaternion.Slerp(targetRotation, initialRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        weapon.localRotation = initialRotation;

        isAttacking = false;
    }

}
