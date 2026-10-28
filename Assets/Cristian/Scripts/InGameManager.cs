using TMPro;
using UnityEngine.InputSystem;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    public static InGameManager Instance { get; private set; }

    [SerializeField] private GameObject textoGameOver; 
    [SerializeField] private PlayerBehaviour jugador; 
    [SerializeField] private GameObject pausePanel;
    private bool isPaused = false;

    private bool gameOver = false;

    [Header("Condiciones de Victoria")]
    [SerializeField] private float duracionTotalNoche = 600f;
    [SerializeField] private TextMeshProUGUI textoReloj; 
    [SerializeField] private GameObject panelVictoria;
    private float tiempoTranscurrido = 0f;
    private int horaActualMostrada = 12;
    private bool partidaTerminada = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        ActualizarTextoReloj(12);
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
            jugador.Morir();
        }

        Debug.Log("¡Los 6 puntos fueron completados! Game Over.");

    }

    private void Update()
    {
        if (partidaTerminada || gameOver) return;

        tiempoTranscurrido += Time.deltaTime;

        float progreso = Mathf.Clamp01(tiempoTranscurrido / duracionTotalNoche);

        int horasPasadas = Mathf.FloorToInt(progreso * 5f);
        int horaCalculada = (horasPasadas == 0) ? 12 : horasPasadas;

        if (horaCalculada != horaActualMostrada)
        {
            horaActualMostrada = horaCalculada;
            ActualizarTextoReloj(horaActualMostrada);
        }

        if (tiempoTranscurrido >= duracionTotalNoche)
        {
            DispararVictoria();
        }
    }

    private void ActualizarTextoReloj(int hora)
    {
        if (textoReloj != null)
        {
            textoReloj.text = $"{hora} AM";
        }
    }

    public void DispararVictoria()
    {
        if (partidaTerminada || gameOver) return;
        partidaTerminada = true;

        ActualizarTextoReloj(5);
        Debug.Log("¡Sobreviviste hasta las 5 AM! Has ganado.");

        if (panelVictoria != null)
        {
            panelVictoria.SetActive(true);
        }
        Time.timeScale = 0f;
    }

    public void Pausar()
    {
        isPaused = true;
        if (pausePanel != null) pausePanel.SetActive(true);
        Time.timeScale = 0f; 

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Reanudar()
    {
        isPaused = false;
        if (pausePanel != null) pausePanel.SetActive(false);
        Time.timeScale = 1f; 

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void IrAlMenuPrincipal()
    {
        Time.timeScale = 1f; 
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

}