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
    [SerializeField] private AudioSource igniteSound;
    [SerializeField] private AudioSource extinguishSound;

    [Header("Settings")]
    [SerializeField] private bool startLit = false;
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] private bool debugMode = true;

    private bool isLit;

    private void Start()
    {
        // Register this torch with the manager
        TorchManager.Instance?.RegisterTorch();
        
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
        if (TorchManager.Instance != null)
        {
            TorchManager.Instance.UnregisterTorch(isLit);
        }
    }

    private void Update()
    {
        //Checamos que el jugador esté cerca de una antorcha
        bool isCloseText = IsPlayerCloseEnough();
        txtInteract.SetActive(isCloseText);


        if (Input.GetKeyDown(KeyCode.E) && IsPlayerCloseEnough())
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
        if (!silent)
        {
            if (isLit) AudioManager.Instance.Play("IgniteTorch");
            else if (!isLit) AudioManager.Instance.Play("IgniteTorch");
        }

        // Notify TorchManager
        TorchManager.Instance?.TorchStateChanged(isLit);
    }

    private bool IsPlayerCloseEnough()
    {
        if (playerTransform == null) return false;
        return Vector3.Distance(transform.position, playerTransform.position) <= interactionDistance;
    }
}