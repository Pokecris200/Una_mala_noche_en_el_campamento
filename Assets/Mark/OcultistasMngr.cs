using UnityEngine;
using System.Collections;

public class OcultistasMngr : MonoBehaviour
{
    public GameObject OcultistaPrefab;
    public float spawnInterval = 3f;
    private bool detenerSpawn = false;


    private void Start()
    {
        StartCoroutine(SpawnLoop());
        
    }
    private IEnumerator SpawnLoop()
    {
        while (!detenerSpawn)
        {
            yield return new WaitForSeconds(spawnInterval);
            if (PuntoSobrenatural.PuntosOcupados()||PuntoSobrenatural.ObtenerPuntoLibre()==null)
            {
                detenerSpawn = true;
                yield break;

            }
            Instantiate(OcultistaPrefab, transform.position, Quaternion.identity); 



        }

    }
    public void DetenerSpawn() => detenerSpawn=true;

}
