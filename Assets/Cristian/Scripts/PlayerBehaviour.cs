using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float speed = 10f;
    private float sprint = 1f;
    Vector2 horizontalInput;
    private bool estaMuerto = false;
    [SerializeField] private Transform cameraPosition;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Sprinting")]
    [SerializeField] private float maxStamina = 50f; //Estamina Maxima
    [SerializeField] private float stamina = 50f; //Estamina actual
    [SerializeField] private float staminaDrain = 15f;    // Puntos de estamina gastados por segundo
    [SerializeField] private float staminaRecovery = 10f;
    [SerializeField] private bool running = false;
    [SerializeField] private Image staminaBarImage;

    [Header("Attack")]
    [SerializeField] private GameObject weapon;
    private Transform weaponTransform;
    private Collider weaponCollider;
    private bool isAttacking = false;

    [Header("Ardilla")]
    [SerializeField] private GameObject ardilla;
    [SerializeField] private float throwForce;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        weaponTransform = weapon.GetComponent<Transform>();
        weaponCollider = weapon.GetComponent<Collider>();
        animator = GetComponent<Animator>();
    }

    void OnMove(InputValue value)
    {
        if (!enabled || estaMuerto || Time.timeScale == 0f)
        {
            horizontalInput = Vector2.zero;
            return;
        }
        horizontalInput = value.Get<Vector2>();
    }

    void OnAttack(InputValue value)
    {
        if (!enabled || estaMuerto || Time.timeScale == 0f) return;
        AttackInstructions();
    }

    /*void OnJump(InputValue value)
    {
        ShootSquirrell(throwForce);
    }*/

    void FixedUpdate()
    {
        if (estaMuerto) return;

        Vector3 targetVelocity = Vector3.zero;

        if (!isAttacking && horizontalInput.sqrMagnitude > 0.01f && cameraPosition != null)
        {
            // Obtener forward y right de la cámara proyectados sobre el plano horizontal
            Vector3 camForward = cameraPosition.forward;
            camForward.y = 0f;
            camForward.Normalize();

            Vector3 camRight = cameraPosition.right;
            camRight.y = 0f;
            camRight.Normalize();

            // Vector de dirección deseado
            Vector3 moveDirection = (camRight * horizontalInput.x) + (camForward * horizontalInput.y);
            if (moveDirection.magnitude > 1f)
            {
                moveDirection.Normalize();
            }

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15f * Time.fixedDeltaTime);

            targetVelocity = moveDirection * speed * sprint;
        }

        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
    }

    private void Update()
    {
        if (estaMuerto) return;
        SprintingInstructions();
        UpdateAnimations();
    }

    void UpdateAnimations()
    {
        if (animator == null) return;

        bool isMoving = !isAttacking && horizontalInput.sqrMagnitude > 0.01f;
        float currentSpeed = isMoving ? horizontalInput.magnitude * sprint : 0f;

        animator.SetFloat("Speed", currentSpeed);
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isAtacking", isAttacking);
    }


    // Sección de Sprint del jugador
    void SprintingInstructions()
    {
        if (Keyboard.current != null)
        {
            running = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;
        }

        bool isMoving = horizontalInput.sqrMagnitude > 0.01f;

        if (running && isMoving && stamina > 0f)
        {
            sprint = 2f;
            stamina -= staminaDrain * Time.deltaTime;

            if (stamina < 0f)
            {
                stamina = 0f;
            }
        }
        else
        {
            sprint = 1f;

            if (!running && stamina < maxStamina)
            {
                stamina += staminaRecovery * Time.deltaTime;
                if (stamina > maxStamina)
                {
                    stamina = maxStamina;
                }
            }
        }

        if (staminaBarImage != null && maxStamina > 0f)
        {
            staminaBarImage.fillAmount = stamina / maxStamina;
        }
    }


    // Seccion Ataque del jugador
    void AttackInstructions()
    {
        if (!isAttacking)
        {
            // Alinear al personaje con el frente de la cámara
            if (cameraPosition != null)
            {
                Vector3 camForward = cameraPosition.forward;
                camForward.y = 0f;
                if (camForward.sqrMagnitude > 0.001f)
                {
                    transform.rotation = Quaternion.LookRotation(camForward);
                }
            }

            isAttacking = true;
        }
    }

    // Llamar justo cuando la sartén empieza el recorrido de ataque
    public void EnableWeaponTrigger()
    {
        isAttacking = true;
        if (weaponCollider != null)
        {
            weaponCollider.isTrigger = true;
        }
    }

    // Llamar cuando termina de dar el golpe hacia adelante
    public void DisableWeaponTrigger()
    {
        isAttacking = false;
        if (weaponCollider != null)
        {
            weaponCollider.isTrigger = false;
        }
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

    public void Morir()
    {
        if (estaMuerto) return;
        estaMuerto = true;

        horizontalInput = Vector2.zero;
        running = false;
        isAttacking = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true; // Evita que siga rebotando o siendo empujado
        }

        DisableWeaponTrigger();

        if (TryGetComponent<Collider>(out var col))
        {
            col.enabled = false;
        }

        if (animator != null)
        {
            animator.SetTrigger("Die"); // Asegúrate de tener el trigger "Die" en el Animator Controller
        }
    }

}
