using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class OcultistasCtrl : MonoBehaviour
{
    public bool llego { get; private set; } = false;
    private NavMeshAgent agente;
    private PuntoSobrenatural objetivoRit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        objetivoRit = PuntoSobrenatural.ObtenerPuntoLibre();
        if (objetivoRit == null)
        {
            Debug.LogError("sin objetivo, destruir");
            Destroy(gameObject);
            return;
        }
        if (!agente.isOnNavMesh)
        {
            Debug.LogError("Sin NavMesh");
            return;

        }
        agente.SetDestination(objetivoRit.transform.position);

        
    }

    // Update is called once per frame
    void Update()
    {
        if (llego)
        {
            return;
        }
        
    }
    public void LlegadaAlPunto(PuntoSobrenatural punto)
    {
        if (llego) return;
        llego = true;
        objetivoRit = punto;

        if (agente != null && agente.isOnNavMesh)
        {
            agente.isStopped = true;

        }
        Debug.Log($"{name} llego a {punto.name }");
        

    }
    private void OnDestroy()
    {
        if (llego && objetivoRit !=null)
        {
            objetivoRit.Liberar();

        }
    }
}
