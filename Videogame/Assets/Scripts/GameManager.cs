using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    //Creamos la instancia de singleton
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI torchCounterText;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private TMP_Text scoreGameOver; //Damos acceso a tanto el objeto que maneja el UI y el texto del score
    [SerializeField] private TMP_Text timeExpended;  //Damos acceso al tiempo invertido en la partida
    [SerializeField] private TMP_Text timerSelected; //Damos acceso al tiempo invertido en la partida


    [Header("Events")]
    public UnityEvent onAllTorchesLit;
    public UnityEvent<int> onTorchCountChanged;

    [Header("Game Over Messages")]
    [SerializeField] private string winMessage = "�Ganaste! �Encendiste todas las luces a tiempo!";
    [SerializeField] private string loseMessage = "�Perdiste! No cerraste los ojos y el espectro te atrap�";

    private int totalTorches = 0;
    private int litTorches = 0;

    private bool isGameStarted = false;                                  //Manejamos un timer para indicar que el juego est� activado
    private bool isGameOver = false;                                     //Manejamos como tal un indicador que el juego est� terminado
    private float cooldownToPlay = 0.5f;
    private float timerToPlay = 0f;

    private float elapsedTime = 0f;             //Tiempo pasado de la partida
    private bool isTimerRunning = false;        //Medidor para determinar que el tiempo pasa

    private void Awake()
    {
        //Manejamos la funci�n awake, para setear el singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); //�sto es para mantener el juego vivo entre escenas, y es opcional
        }
        else
        {
            //Si ya existe el objeto del singleton en una escena, no se ocupa agregar otro, lo destruimos
            Destroy(gameObject);
        }
    }

    void Start()
    {
        //Inicializamos las estad�sticas del juego
        timerToPlay = cooldownToPlay;
    }

    private void Update()
    {
        //Primer timer para indicar que el juego puede iniciar
        if (!isGameStarted)
        {
            if (timerToPlay > 0)
            {
                timerToPlay -= Time.deltaTime;
            }
            else
            {
                //Spawneamos el juego
                StartLevel();
            }
        }

        //Actualizamos el timer si el juego contin�a
        if (isTimerRunning && !isGameOver)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    //Funci�n encargada de iniciar la partida por nivel
    private void StartLevel()
    {
        //Juego se inici�
        isGameStarted = true;
        isTimerRunning = true;
        elapsedTime = 0f;           //Reseteamos el timer cuando se empieza el juego de nuevo
    }

    //Funci�n encargada de determinar que el juego termin�
    public void GameOver()
    {
        //Activamos el men� de gameover si �ste existe
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }

        //Actualizamos el puntaje del game score
        if (timeExpended != null)
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);
            timeExpended.text = $"Tiempo sobrevivido: {minutes:00}:{seconds:00} s";
        }

        //Pausamos el juego del todo
        Time.timeScale = 0f;
        Debug.Log("Game Over");
    }
    public void ResetGame()
    {
       
    }
    public void AssignUIReferences(GameObject gameOverScreen, TMP_Text scoreGameOver)
    {
        //Asignamos la referencia de los objetos de gameover y asignar el score
        this.gameOverScreen = gameOverScreen;
        this.scoreGameOver = scoreGameOver;
    }


    //Secci�n de antorchas
    public void TorchStateChanged(bool isLit)
    {
        litTorches += isLit ? 1 : -1;
        litTorches = Mathf.Clamp(litTorches, 0, totalTorches);
        UpdateUI();

        if (AllTorchesAreLit)
        {
            AllTorchesLit();
        }
    }

    private void UpdateUI()
    {
        if (torchCounterText != null)
        {
            torchCounterText.text = $"{litTorches}/{totalTorches}";
        }

        onTorchCountChanged?.Invoke(litTorches);
    }
    private void UpdateTimerDisplay()
    {
        if (timeExpended != null)
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);
            timerSelected.text = $"{minutes:00}:{seconds:00}";
        }
    }

    private void AllTorchesLit()
    {
        Debug.Log("YOU WIN! All torches are lit!");
        scoreGameOver.text = winMessage;
        GameOver();
        onAllTorchesLit?.Invoke();
    }

    public void PlayerDied()
    {
        Debug.Log("YOU LOSE! You were catched by the demon!");
        scoreGameOver.text = loseMessage;
        GameOver();
    }

    public void RegisterTorch()
    {
        totalTorches++;
        UpdateUI();
    }

    public void UnregisterTorch(bool wasLit)
    {
        totalTorches = Mathf.Max(0, totalTorches - 1);
        if (wasLit)
        {
            litTorches--;
        }
        UpdateUI();
    }

    // Helper properties
    public int LitTorches => litTorches;
    public int TotalTorches => totalTorches;
    public bool AllTorchesAreLit => totalTorches > 0 && litTorches >= totalTorches;
}
