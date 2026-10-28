using UnityEngine;
using TMPro; // Si usas TextMeshPro, o UnityEngine.UI si usas Text clásico

public class InGameManager : MonoBehaviour
{
    public static InGameManager Instance { get; private set; }

    [SerializeField] private GameObject textoGameOver; // El objeto con el texto en tu Canvas
    [SerializeField] private PlayerBehaviour jugador;  // O buscarlo por tag

    private bool gameOver = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void DispararGameOver()
    {
        if (gameOver) return;
        gameOver = true;

        if (textoGameOver != null)
        {
            textoGameOver.SetActive(true);
        }

        // 2. Eliminar o matar al jugador
        if (jugador != null)
        {
            // Si tu PlayerBehaviour tiene un método para morir, llámalo aquí:
            // jugador.Morir();
            // O directamente destruirlo / desactivarlo:
            Destroy(jugador.gameObject);
        }

        Debug.Log("¡Los 6 puntos fueron completados! Game Over.");
    }
}