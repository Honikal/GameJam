using UnityEngine;

public class Antorcha : MonoBehaviour
{
    [Header("Visual Effects")]
    [SerializeField] private Light torchLight;
    [SerializeField] private ParticleSystem fireParticles;
    [SerializeField] private AudioSource igniteSound;
    [SerializeField] private AudioSource extinguishSound;

    [Header("Settings")]
    [SerializeField] private bool startLit = false;
    [SerializeField] private KeyCode toggleKey = KeyCode.None; // Optional keyboard control

    private bool isLit = false;
    private Collider interactionCollider;

    private void Awake()
    {
        interactionCollider = GetComponent<Collider>();
        SetTorchState(startLit, silent: true);
    }

    private void Update()
    {
        // Optional keyboard control for testing
        if (toggleKey != KeyCode.None && Input.GetKeyDown(toggleKey))
        {
            ToggleTorch();
        }
    }

    public void ToggleTorch()
    {
        SetTorchState(!isLit);
    }

    public void SetTorchState(bool lit, bool silent = false)
    {
        if (isLit == lit) return;

        isLit = lit;

        // Visual effects
        if (torchLight != null) torchLight.enabled = isLit;
        if (fireParticles != null)
        {
            if (isLit) fireParticles.Play();
            else fireParticles.Stop();
        }

        // Audio effects
        if (!silent)
        {
            if (isLit && igniteSound != null) igniteSound.Play();
            else if (!isLit && extinguishSound != null) extinguishSound.Play();
        }

        // Notify TorchManager
        if (TorchManager.Instance != null)
        {
            TorchManager.Instance.TorchStateChanged(isLit);
        }
    }

    // For player interaction (attach to collider)
    private void OnMouseDown()
    {
        if (IsPlayerCloseEnough()) // Implement your distance check
        {
            ToggleTorch();
        }
    }

    private bool IsPlayerCloseEnough()
    {
        // Implement your player distance check here
        // Example: return Vector3.Distance(transform.position, player.transform.position) < 3f;
        return true; // Placeholder
    }

    // For trigger-based interaction
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetButtonDown("Interact"))
        {
            ToggleTorch();
        }
    }
}