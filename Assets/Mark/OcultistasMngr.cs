
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OcultistasMngr : MonoBehaviour
{
    public GameObject OcultistaPrefab;
    public float spawnInterval = 3f;
    private bool detenerSpawn = false;
    public List<Transform> puntosDeSpawn = new List<Transform>();


    private void Start()
    {
        if (puntosDeSpawn == null || puntosDeSpawn.Count == 0)
        {
            Debug.LogError($"{name} no hay puntos asignados");
            detenerSpawn = true;
            return;
        }

        StartCoroutine(SpawnLoop());

    }
    private IEnumerator SpawnLoop()
    {
        while (!detenerSpawn)
        {
            yield return new WaitForSeconds(spawnInterval);
            if (PuntoSobrenatural.PuntosOcupados()||PuntoSobrenatural.ObtenerPuntoLibre()==null)
            {
                continue;

            }
            Transform puntoElegido = puntosDeSpawn[Random.Range(0, puntosDeSpawn.Count)];
            Instantiate(OcultistaPrefab, puntoElegido.position, puntoElegido.rotation); 



        }

    }
    public void DetenerSpawn() => detenerSpawn=true;

}
