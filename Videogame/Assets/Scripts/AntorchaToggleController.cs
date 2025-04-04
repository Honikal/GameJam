using UnityEngine;

public class Antorcha : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject circulo_0; // Drag the child object here

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

    private void Awake()
    {
        // If circulo_0 isn't assigned, try to find it automatically
        if (circulo_0 == null)
        {
            circulo_0 = transform.Find("circulo_0")?.gameObject;
            if (debugMode && circulo_0 != null) 
                Debug.Log("Found circulo_0 automatically", this);
        }

        SetTorchState(startLit, silent: true);
    }

    private void Update()
    {
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

        // Toggle child object
        if (circulo_0 != null)
        {
            circulo_0.SetActive(isLit); // Simply toggle active state
            if (debugMode) Debug.Log($"circulo_0 active: {isLit}");
        }
        else if (debugMode)
        {
            Debug.LogWarning("circulo_0 reference is missing!", this);
        }

        // Toggle other effects
        if (torchLight != null) torchLight.enabled = isLit;
        if (fireParticles != null)
        {
            if (isLit) fireParticles.Play();
            else fireParticles.Stop();
        }

        if (!silent)
        {
            if (isLit && igniteSound != null) igniteSound.Play();
            else if (!isLit && extinguishSound != null) extinguishSound.Play();
        }
    }

    private bool IsPlayerCloseEnough()
    {
        if (playerTransform == null) return false;
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        return distance <= interactionDistance;
    }
}