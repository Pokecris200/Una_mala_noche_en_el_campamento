using UnityEngine;
using System.Collections.Generic;

public class PuntoSobrenatural : MonoBehaviour
{
    public bool estaLibre = true;
    //lista para verificar los puntos

    public static readonly List<PuntoSobrenatural> listaPuntos = new List<PuntoSobrenatural>();
    //no se como funciona, preguntar
    private void OnEnable()
    {
        if (!listaPuntos.Contains(this))
        {
            listaPuntos.Add(this);

        }
    }
    private void OnDisable()
    {
        listaPuntos.Remove(this);
    }
    //Indica si hay puntos libres o no

    public static PuntoSobrenatural ObtenerPuntoLibre()
    {
        //crea una lista temporal de puntos libres
        List<PuntoSobrenatural> libre = new List<PuntoSobrenatural>();
        foreach (var l in listaPuntos)
        {
            if (l.estaLibre)
            {
                libre.Add(l);

            }

        }
        if (libre.Count == 0)
        {
            return null;
        }
        return libre[Random.Range(0, libre.Count)];

    }
    //indica si los puntos están ocupados
    public static bool PuntosOcupados()
    {
        foreach (var l in listaPuntos)
        {
            if (l.estaLibre)
            {
                return false;
            }

        }
        return listaPuntos.Count > 0;
        //con la lista en cero se activa la condición de puntos libres
    }
    public void Reservar()
    {
        estaLibre = false;
        Debug.Log($"{name} reservo un punto, puntos libres:{ContadorDeLibres()}");
    }
    public void Reclamar()
    {
        estaLibre = false;
        Debug.Log($"{name} reclamo un punto, puntos libres: {ContadorDeLibres()}");
    }
    public void Liberar()
    {
        estaLibre = true;
    }
    private static int ContadorDeLibres()
    {
        int n = 0;
        foreach (var l in listaPuntos) if (l.estaLibre) n++;
        return n;
        

    }
    private void OnTriggerEnter(Collider other)
    {
        OcultistasCtrl ocultista = other.GetComponentInParent<OcultistasCtrl>();
        if (ocultista !=null && !ocultista.llego)
        {
            if (!ocultista.ReservoTalPunto(this))
            {
                Destroy(ocultista.gameObject);
                return;
            }
            ocultista.LlegadaAlPunto(this);

        }


        //if (!estaLibre)
        //{
        //    return;
        //}
        //OcultistasCtrl ocultista = other.GetComponentInParent<OcultistasCtrl>();
        //if (ocultista !=null && !ocultista.llego)
        //{
        //    Reclamar();
        //    ocultista.LlegadaAlPunto(this);

        //}
    }


}
