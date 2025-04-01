using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    //Creamos la instancia de singleton
    public static GameManager Instance { get; private set; }

    public List<GameObject> activeMonsters = new List<GameObject>();    //Manejamos una lista de los asteroides activos en el sistema
    public GameObject activePlayer;                                     //Manejamos el jugador dentro del sistema

    public List<GameObject> activeCandles = new List<GameObject>();     //Manejamos la lista de candelas activas
    public List<GameObject> activeObjects = new List<GameObject>();     //Objetos para aumentar de vuelta la cordura

    public bool isGameStarted = false;                                  //Manejamos un timer para indicar que el juego está activado
    public bool isGameOver = false;                                     //Manejamos como tal un indicador que el juego está terminado
    private float cooldownToPlay = 0.5f;
    private float timerToPlay = 0f;


    //Definimos eventos para modificar el puntaje y la vida
    public event System.Action<int> OnInsanityChanged;
    public event System.Action<int> OnHealthChanged;

    //Damos acceso a tanto el objeto que maneja el UI y el texto del score
    public GameObject gameOverScreen;
    public TMP_Text scoreGameOver;

    //Modificamos las propiedades de vida y puntaje para activar eventos
    private int _insanity;
    public int insanity     //Porcentaje de sanidad o mantener la visión encendida
    {
        get => _insanity;
        set
        {
            _insanity = value;
            OnInsanityChanged?.Invoke(_insanity); //Trigger cuando el puntaje cambie
        }
    }

    private int _health;    //Porcentaje de vida actual del jugador
    public int health
    {
        get => _health;
        set
        {
            _health = value;
            OnHealthChanged?.Invoke(_health); //Trigger cuando la vida cambie
        }
    }

    private int _time;      //Tiempo de duración de la partida
    public int time
    {
        get => _time;
        set
        {
            _time = value;
            OnHealthChanged?.Invoke(_time); //Trigger cuando la vida cambie
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
            Debug.Log("Activamos menú de GameOver");
            gameOverScreen.SetActive(true);
        }

        //Actualizamos el puntaje del game score
        if (scoreGameOver != null)
        {
            scoreGameOver.text = $"Tiempo que duraste vivo: {time}";
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
}
