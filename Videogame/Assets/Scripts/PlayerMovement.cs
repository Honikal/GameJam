
using System;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{ 
    [SerializeField] float maxSanity = 100f;  //Manejamos la sanidad o el poder parpadear
    [SerializeField] float maxHealth = 100f;  //Manejamos el sistema de salud del jugador, que se reduce cuando observamos al monstruo por mucho tiempo
    //Tiempo que toma que el jugador pierda la cordura completamente
    [SerializeField] float eyesClosedDuration = 20f;    //Toma 20 segundos para perder la cordura
    [SerializeField] float healthDuration = 5f;    //Toma 5 segundos para perder la salud completa
    [SerializeField] float healthRecoveryDuration = 30f; //Toma 30 segundos para poder recuperarse la salud de forma completa

    //Por si tocara hacer regeneración propia, regeneración por segundo cuando los ojos están abiertos
    //[SerializeField] float eyesOpenRegenRate = 15f;

    [Header("Referencias")]
    public Image SanityBar;
    public Image HealthBar;
    [SerializeField] private GameObject circulo_0; // The child circle to toggle
    public float moveSpeed;

    private float currentSanity = 100f;
    private float currentHealth = 100f;
    private float sanityDrainRate;
    private float healthDrainRate;
    private float healthRegainRate;
    private Rigidbody2D rb;
    private CircleCollider2D visionTrigger;
    private bool isEyesClosed = false;
    private bool isMonsterInSight = true;
    private Vector2 moveDirection;


    private void Start()
    {
        //Seteamos los valores iniciales
        rb = GetComponent<Rigidbody2D>();
        visionTrigger = GetComponent<CircleCollider2D>();
        currentSanity = maxSanity;
        currentHealth = maxHealth;

        //Calculamos el rango de pérdida de sanidad basado en el tiempo deseado
        sanityDrainRate = maxSanity / eyesClosedDuration;
        healthDrainRate = maxHealth / healthDuration;
        healthRegainRate = maxHealth / healthRecoveryDuration;
    }

    void Update()
    {
        ProccesInputs();
        //HandleSanity();
        HandleHealth();
        UpdateUI();
    }

    void FixedUpdate()
    {
        Move();
    }


    void ProccesInputs() {
        //Manejamos el input de movimiento
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(moveX, moveY).normalized;

        //Checamos que se presione el botón space
        if (Input.GetKeyDown(KeyCode.Space)) ToggleEyes(true);
        if (Input.GetKeyUp(KeyCode.Space)) ToggleEyes(false);
    }

    void ToggleEyes(bool value)
    {
        //Activamos el círculo del jugador
        isEyesClosed = value;
        circulo_0.SetActive(!value);
        //Acá también debemos de desactivar o activar el trigger
        visionTrigger.enabled = !value;
        Debug.Log(value ? "Ojos cerrados" : "Ojos abiertos");
    }
    void Move()
    {
        rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
    }

    private void HandleSanity()
    {
        if (isEyesClosed)
        {
            //Reducimos la sanidad a base de tiempo
            currentSanity -= sanityDrainRate * Time.deltaTime;
            Debug.Log($"Perdemos sanidad, sanidad actual: {currentSanity}");
        }
        //Nos encargamos de mantener la sanidad en los límites
        currentSanity = Mathf.Clamp(currentSanity, 0, maxSanity);
    }

    private void HandleHealth()
    {
        //Mediante ésta función, checamos la existencia que hayamos visto al monstruo, y checamos el caso de que tengamos los ojos abiertos
        if (isMonsterInSight)
        {
            if (!isEyesClosed)
            {
                currentHealth -= healthDrainRate * Time.deltaTime;
                Debug.Log($"Perdemos salud, salud actual: {currentHealth}");
            }
            else
            {
                currentHealth += healthRegainRate * Time.deltaTime;
                Debug.Log($"Recuperamos salud, salud actual: {currentHealth}");
            }

        }
        //Nos encargamos de mantener la salud en los límites
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public void IncreaseSanity(float amount)
    {
        currentSanity = Mathf.Clamp(currentSanity + amount, 0, maxSanity);
    }

    private void UpdateUI()
    {
        //Solo actualizamos cuando hay cambios
        float normalizedSanity = currentSanity / maxSanity;
        float normalizedHealth = currentHealth / maxHealth;

        Debug.Log($"Tamaño a pintar del campo de sanidad: {normalizedSanity}");
        Debug.Log($"Tamaño a pintar del campo de salud: {normalizedHealth}");
        SanityBar.fillAmount = normalizedSanity;
        HealthBar.fillAmount = normalizedHealth;
    }

    //El monstruo entra en el área de visibilidad del jugador
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Primero checamos que el valor con el que chocamos es el monstruo
        if (collision.CompareTag("Enemy"))
        {
            isMonsterInSight = true;
        }
    }

    //El monstruo sale del área de visibilidad del jugador
    private void OnTriggerExit2D(Collider2D collision)
    {
        //Primero checamos que el monstruo haya salido del área
        if (collision.CompareTag("Enemy"))
        {
            isMonsterInSight = false;
        }
    }



} 
