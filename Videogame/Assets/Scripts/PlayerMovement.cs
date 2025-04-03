
using System;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.Rendering.DebugUI;

public class Movement : MonoBehaviour
{ 
    [SerializeField] float maxClosedEyes = 100f;  //Manejamos la sanidad o el poder parpadear
    [SerializeField] float maxHealth = 100f;  //Manejamos el sistema de salud del jugador, que se reduce cuando observamos al monstruo por mucho tiempo
    //Tiempo que toma que el jugador pierda la cordura completamente
    [SerializeField] float eyesClosedCooldown = 3f;    //El jugador puede mantener los ojos cerrados por 5 segundos
                                                       //Cooldown para determinar el tiempo de ojos cerrados
    [SerializeField] float healthDuration = 5f;    //Toma 5 segundos para perder la salud completa

    //Por si tocara hacer regeneración propia, regeneración por segundo cuando los ojos están abiertos
    //[SerializeField] float eyesOpenRegenRate = 15f;

    [Header("Referencias")]
    public TextMeshProUGUI timerValue;
    public Image EyesBar;
    public Image HealthBar;
    [SerializeField] private GameObject circulo_0; // The child circle to toggle
    public float moveSpeed;

    private float currentClosedEyes = 100f;
    private float currentHealth = 100f;
    private float closeEyesRate;
    private float healthDrainRate;
    private float cooldownTimer = 0f;
    private Rigidbody2D rb;
    private CircleCollider2D visionTrigger;
    private bool isEyesClosed = false;
    private bool isMonsterInSight = false;
    private bool isCooldownActive = false;
    private Vector2 moveDirection;


    private void Start()
    {
        //Seteamos los valores iniciales
        rb = GetComponent<Rigidbody2D>();
        visionTrigger = GetComponent<CircleCollider2D>();
        currentHealth = maxHealth;
        currentClosedEyes = maxClosedEyes;

        //Calculamos el rango de pérdida de sanidad basado en el tiempo deseado
        healthDrainRate = maxHealth / healthDuration;
        closeEyesRate = maxClosedEyes / eyesClosedCooldown;
    }

    void Update()
    {
        ProccesInputs();
        HandleEyeTimer();
        HandleCooldown();
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
        if (Input.GetKeyDown(KeyCode.Space)) TryCloseEyes();
        if (Input.GetKeyUp(KeyCode.Space)) OpenEyes();
    }

    void TryCloseEyes()
    {
        if (!isCooldownActive && currentClosedEyes > 0)
        {
            isEyesClosed = true;
            circulo_0.SetActive(false);
            visionTrigger.enabled = false;
            Debug.Log("Ojos cerrados");
        }
    }
    void OpenEyes()
    {
        //Sé que solo sucederá cuando se suelte el botón pero igual
        if (isEyesClosed)
        {
            isEyesClosed = false;
            circulo_0.SetActive(true);
            visionTrigger.enabled = true;
            StartCooldown();
            Debug.Log("Ojos abiertos");
        }
    }

    void Move()
    {
        rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
    }

    private void HandleHealth()
    {
        //Mediante ésta función, checamos la existencia que hayamos visto al monstruo, y checamos el caso de que tengamos los ojos abiertos
        if (isMonsterInSight && !isEyesClosed)
        {
            currentHealth -= healthDrainRate * Time.deltaTime;
            Debug.Log($"Perdemos salud, salud actual: {currentHealth}");
        }
        //Nos encargamos de mantener la salud en los límites
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    private void HandleEyeTimer()
    {
        //Si mantenemos los ojos cerrados tenemos un cooldown hasta poder volver a abrir los ojos
        if (isEyesClosed)
        {
            currentClosedEyes -= closeEyesRate * Time.deltaTime;

            //Si se llegara al punto en el que la barra de ojos cerrados está afectando, entonces
            if (currentClosedEyes <= 0)
            {
                //Cuando llegue a 0, igual le abriremos los ojos
                currentClosedEyes = 0;
                OpenEyes();
            }
        }
        //Mantenemos los estados de ojos cerrados al límite
        currentClosedEyes = Math.Clamp(currentClosedEyes, 0, maxClosedEyes);
    }

    private void StartCooldown()
    {
        isCooldownActive = true;
        cooldownTimer = eyesClosedCooldown;
    }
    private void HandleCooldown()
    {
        if (isCooldownActive)
        {
            //Reducimos el timer en base a segundos reales
            cooldownTimer -= Time.deltaTime;
            currentClosedEyes += (maxClosedEyes / eyesClosedCooldown) * Time.deltaTime;

            //Si el cooldown está en 0, entonces...
            if (cooldownTimer <= 0f)
            {
                //Podemos cerrar ojos de nuevo
                isCooldownActive = false;
                currentClosedEyes = maxClosedEyes;
            }
        }
    }

    public void IncreaseHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log($"Health Restored: {currentHealth}");
    }

    private void UpdateUI()
    {
        //Solo actualizamos cuando hay cambios
        float normalizedSight = currentClosedEyes / maxClosedEyes;
        float normalizedHealth = currentHealth / maxHealth;

        Debug.Log($"Tamaño a pintar del campo de salud: {normalizedHealth}");
        EyesBar.fillAmount = normalizedSight;
        HealthBar.fillAmount = normalizedHealth;

        //Actualizamos la medida de los ojos
        EyesUI();
    }

    private void EyesUI()
    {
        //Actualizamos el timer basado en el texto
        if (isCooldownActive)
        {
            timerValue.text = $"Cooldown: {cooldownTimer.ToString("0.0")}s";
        } else if (isEyesClosed)
        {
            timerValue.text = $"Close time: {currentClosedEyes.ToString("0.0")}s";
        } else
        {
            timerValue.text = $"Ready: {currentClosedEyes.ToString("0.0")}s";
        }
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
