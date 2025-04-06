using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //Creamos la instancia de singleton
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI torchCounterText;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private TMP_Text scoreGameOver; //Damos acceso a tanto el objeto que maneja el UI y el texto del score
    [SerializeField] private TMP_Text timeExpended;  //Damos acceso al tiempo invertido en la partida

    [Header("Events")]
    public UnityEvent onAllTorchesLit;
    public UnityEvent<int> onTorchCountChanged;

    [Header("Game Over Messages")]
    [SerializeField] private string winMessage = "¡Ganaste! ¡Encendiste todas las luces a tiempo!";
    [SerializeField] private string loseMessage = "¡Perdiste! No cerraste los ojos y el espectro te atrapó";

    private int totalTorches = 0;
    private int litTorches = 0;

    private bool isGameStarted = false;                                  //Manejamos un timer para indicar que el juego está activado
    private bool isGameOver = false;                                     //Manejamos como tal un indicador que el juego está terminado
    private float cooldownToPlay = 0.5f;
    private float timerToPlay = 0f;

    //Definimos eventos para modificar el puntaje y la vida
    public event System.Action<int> OnTimeChanged;

    private int _time;      //Tiempo de duración de la partida
    public int time
    {
        get => _time;
        set
        {
            _time = value;
            OnTimeChanged?.Invoke(_time); //Trigger cuando la vida cambie
        }
    }

    private void Awake()
    {
        //Manejamos la función awake, para setear el singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); //Ésto es para mantener el juego vivo entre escenas, y es opcional
        }
        else
        {
            //Si ya existe el objeto del singleton en una escena, no se ocupa agregar otro, lo destruimos
            Destroy(gameObject);
        }
    }

    void Start()
    {
        //Inicializamos las estadísticas del juego
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
    }

    //Función encargada de iniciar la partida por nivel
    private void StartLevel()
    {
        //Juego se inició
        isGameStarted = true;
    }

    //Función encargada de determinar que el juego terminó
    public void GameOver()
    {
        //Activamos el menú de gameover si éste existe
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }

        //Actualizamos el puntaje del game score
        /*
        if (scoreGameOver != null)
        {
            scoreGameOver.text = $"Tiempo que duraste vivo: {time}";
        }
        */

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


    //Sección de antorchas
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
