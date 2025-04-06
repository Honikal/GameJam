using UnityEngine;

public class Antorcha : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject circulo_0;
    [SerializeField] private GameObject txtInteract;

    [Header("Visual Effects")]
    [SerializeField] private Light torchLight;
    [SerializeField] private ParticleSystem fireParticles;
    private Animator torchAnimation;

    [Header("Settings")]
    [SerializeField] private bool startLit = false;
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] private bool debugMode = true;

    private bool isLit;
    private bool wasCloseLastFrame;

    private void Start()
    {
        //Extraemos al animador
        torchAnimation = GetComponent<Animator>();

        // Register this torch with the manager
        GameManager.Instance?.RegisterTorch();
        
        // Initialize state
        if (circulo_0 == null)
        {
            circulo_0 = transform.Find("circulo_0")?.gameObject;
        }

        SetTorchState(startLit, silent: true);

    }

    private void OnDestroy()
    {
        // Unregister when destroyed
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UnregisterTorch(isLit);
        }
    }

    private void Update()
    {
        bool isClose = IsPlayerCloseEnough();

        //Solo actualizar cuando el estado de proximidad cambia
        if (isClose != wasCloseLastFrame)
        {
            txtInteract.SetActive(isClose);
            wasCloseLastFrame = isClose;
        }

        //Check para interactuar con el objeto
        if (isClose && Input.GetKeyDown(KeyCode.E))
        {
            ToggleTorch();
        }
    }

    private void ToggleTorch()
    {
        SetTorchState(!isLit);
    }

    private void SetTorchState(bool lit, bool silent = false)
    {
        if (isLit == lit) return;
        isLit = lit;

        // Toggle visual elements
        if (circulo_0 != null) circulo_0.SetActive(isLit);
        if (torchLight != null) torchLight.enabled = isLit;
        if (fireParticles != null)
        {
            if (isLit) fireParticles.Play();
            else fireParticles.Stop();
        }

        // Play sounds
        if (isLit) AudioManager.Instance.Play("IgniteTorch");
        else if (!isLit) AudioManager.Instance.Play("ExtinguishTorch");

        // Notify TorchManager
        GameManager.Instance?.TorchStateChanged(isLit);

        //Actualizamos la animación
        torchAnimation.SetBool("isLit", isLit);
    }

    private bool IsPlayerCloseEnough()
    {
        if (playerTransform == null) return false;
        return Vector3.Distance(transform.position, playerTransform.position) <= interactionDistance;
    }

    
}