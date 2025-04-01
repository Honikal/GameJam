
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

    //Por si tocara hacer regeneración propia, regeneración por segundo cuando los ojos están abiertos
    //[SerializeField] float eyesOpenRegenRate = 15f;

    [Header("Referencias")]
    public Image SanityBar;
    [SerializeField] private GameObject circulo_0; // The child circle to toggle
    public float moveSpeed;

    private float currentSanity = 100f;
    private float currentHealth = 100f;
    private float sanityDrainRate;
    private Rigidbody2D rb;
    private bool isEyesClosed = false;
    private Vector2 moveDirection;


    private void Start()
    {
        //Seteamos los valores iniciales
        rb = GetComponent<Rigidbody2D>();
        currentSanity = maxSanity;
        currentHealth = maxHealth;

        //Calculamos el rango de pérdida de sanidad basado en el tiempo deseado
        sanityDrainRate = maxSanity / eyesClosedDuration;
    }

    void Update()
    {
        ProccesInputs();
        HandleSanity();
        UpdateSanityUI();
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

    public void IncreaseSanity(float amount)
    {
        currentSanity = Mathf.Clamp(currentSanity + amount, 0, maxSanity);
    }

    private void UpdateSanityUI()
    {
        //Solo actualizamos cuando hay cambios
        float normalizedSanity = currentSanity / maxSanity;

        Debug.Log($"Tamaño a pintar del campo de sanidad: {normalizedSanity}");
        SanityBar.fillAmount = normalizedSanity;
    }

} 
