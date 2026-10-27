using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float speed = 10f;
    private float sprint = 1f;
    Vector2 horizontalInput;
    [SerializeField] private Transform cameraPosition;

    [Header("Sprinting")]
    [SerializeField] private float maxStamina = 50f; //Estamina Maxima
    [SerializeField] private float stamina = 50f; //Estamina actual
    [SerializeField] private float staminaDrain = 15f;    // Puntos de estamina gastados por segundo
    [SerializeField] private float staminaRecovery = 10f;
    [SerializeField] private bool running = false;

    [Header("Attack")]
    [SerializeField] private GameObject weapon;
    private Transform weaponTransform;
    private Collider weaponCollider;
    [SerializeField] private float duration = 0.15f; // Tiempo que tarda en ir (y en volver)
    private bool isAttacking = false;

    [Header("Ardilla")]
    [SerializeField] private GameObject ardilla;
    [SerializeField] private float throwForce;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        weaponTransform = weapon.GetComponent<Transform>();
        weaponCollider = weapon.GetComponent<Collider>();
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

    void OnJump(InputValue value)
    {
        ShootSquirrell(throwForce);
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


    // Sección de Sprint del jugador
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


    // Seccion Ataque del jugador
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
        weaponCollider.isTrigger = true;

        Quaternion initialRotation = weaponTransform.localRotation;
        Quaternion targetRotation = initialRotation * Quaternion.Euler(45, 0, 0);

        // 1. Rotar hacia el objetivo
        float elapsed = 0f;
        while (elapsed < duration)
        {
            weaponTransform.localRotation = Quaternion.Slerp(initialRotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        weaponTransform.localRotation = targetRotation;

        // 2. Regresar a la posición original
        elapsed = 0f;
        while (elapsed < duration)
        {
            weaponTransform.localRotation = Quaternion.Slerp(targetRotation, initialRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        weaponTransform.localRotation = initialRotation;

        weaponCollider.isTrigger = false;
        isAttacking = false;
    }

    // Sección ataque de ardilla
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ardilla"))
        {
            ardilla = collision.gameObject;

            if (ardilla.TryGetComponent<Rigidbody>(out Rigidbody ardillaRb))
            {
                ardillaRb.isKinematic = true;
            }
            if (ardilla.TryGetComponent<Collider>(out Collider ardillaCol))
            {
                ardillaCol.enabled = false;
            }
            ardilla.transform.SetParent(cameraPosition);
            ardilla.transform.localPosition = new Vector3(-0.5f,-0.2f,1f);
            ardilla.transform.rotation = Quaternion.identity;
        }
    }

    private void ShootSquirrell(float localTrhowForce)
    {
        if (ardilla == null) return;

        ardilla.transform.SetParent(null);

        Rigidbody ardillaRb = ardilla.GetComponent<Rigidbody>();
        Collider ardillaCol = ardilla.GetComponent<Collider>();
        Collider playerCol = GetComponent<Collider>();

        if (ardillaCol != null) ardillaCol.enabled = true;

        if (ardillaRb != null)
        {
            ardillaRb.isKinematic = false;
            ardillaRb.AddForce(cameraPosition.forward * localTrhowForce, ForceMode.Impulse);
        }

        ardilla = null;
    }

}
