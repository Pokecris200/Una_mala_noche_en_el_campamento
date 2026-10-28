using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class OcultistasCtrl : MonoBehaviour
{
    public bool llego { get; private set; } = false;
    private NavMeshAgent agente;
    private PuntoSobrenatural objetivoRit;

    [Header("Animaciones")]
    [SerializeField] private Animator animator;
    private static readonly int ParamSpeed = Animator.StringToHash("speed");
    private static readonly int ParamIsHurt = Animator.StringToHash("isHurt");
    private static readonly int ParamLive = Animator.StringToHash("live");

    [Header("Combate")]
    public float vida = 3;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (animator != null)
        {
            animator.SetFloat(ParamLive, vida);
        }

        objetivoRit = PuntoSobrenatural.ObtenerPuntoLibre();
        if (objetivoRit == null)
        {
            Debug.LogError("sin objetivo, destruir");
            Destroy(gameObject);
            return;
        }

        objetivoRit.Reservar();
        if (!agente.isOnNavMesh)
        {
            Debug.LogError("Sin NavMesh");
            objetivoRit.Liberar();
            Destroy(gameObject);
            return;

        }
        agente.SetDestination(objetivoRit.transform.position);

        
    }

    // Update is called once per frame
    void Update()
    {
        if (animator == null || vida <= 0) return;

        if (llego || !agente.isOnNavMesh || agente.isStopped)
        {
            animator.SetFloat(ParamSpeed, 0f);
            return;
        }

        float velocidadActual = (agente.velocity.sqrMagnitude > 0.05f) ? 1f : 0f;
        animator.SetFloat(ParamSpeed, velocidadActual);
    }

    public bool ReservoTalPunto(PuntoSobrenatural punto)
    {
        return objetivoRit == punto;

    }
    public void LlegadaAlPunto(PuntoSobrenatural punto)
    {
        if (llego || vida <= 0)
        {
            return;
        }
        if (objetivoRit != punto)
        {
            Debug.LogWarning($"{name}: llego a un punto");
            Destroy(gameObject);
            return;
        }
        llego = true;
        if (agente !=null && agente.isOnNavMesh)
        {
            agente.isStopped = true;
            Debug.Log($"{name} llego a {punto.name}");

        }
        if (animator != null)
        {
            animator.SetFloat(ParamSpeed, 0f); // Regresa a Praying
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (vida <= 0) return;

        if (other.CompareTag("Arma"))
        {
            RecibirDano(1f);
        }
    }
    private void RecibirDano(float cantidad)
    {
        vida -= cantidad;

        if (animator != null)
        {
            animator.SetFloat(ParamLive, vida);

            if (vida <= 0)
            {
                Morir();
                return;
            }

            animator.SetBool(ParamIsHurt, true);
            Invoke(nameof(FinAnimacionGolpe), 0.4f);
        }
    }
    private void FinAnimacionGolpe()
    {
        if (animator != null && vida > 0)
        {
            animator.SetBool(ParamIsHurt, false);
        }
    }

    private void Morir()
    {
        if (agente != null && agente.isOnNavMesh)
        {
            agente.isStopped = true;
        }

        Destroy(gameObject, 3f);
    }



    private void OnDestroy()
    {
        if (objetivoRit !=null)
        {
            objetivoRit.Liberar();

        }
    }
}
