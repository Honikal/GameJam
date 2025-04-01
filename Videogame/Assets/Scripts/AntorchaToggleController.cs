using UnityEngine;

public class AntorchaToggleController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float interactionRadius = 2f;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    
    [Header("References")]
    [SerializeField] private GameObject circulo_0; // The child circle to toggle
    [SerializeField] private GameObject interactionPrompt; // "Press E" UI
    
    private bool playerInRange = false;
    private CircleCollider2D interactionZone;

    void Awake()
    {
        // Set up the persistent interaction zone
        interactionZone = gameObject.AddComponent<CircleCollider2D>();
        interactionZone.radius = interactionRadius;
        interactionZone.isTrigger = true;
        
        // Make sure we have references
        if (circulo_0 == null)
        {
            // Auto-find the child if not assigned
            circulo_0 = transform.Find("circulo_0").gameObject;
        }
        
        // Initialize visibility
        UpdateVisuals();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            UpdateVisuals();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            UpdateVisuals();
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactionKey))
        {
            ToggleCircle();
        }
    }

    void ToggleCircle()
    {
        // Toggle active state
        circulo_0.SetActive(!circulo_0.activeSelf);
        UpdateVisuals();
        
        // Play sound effect if you have one
        // AudioManager.Instance.Play(circulo_0.activeSelf ? "On" : "Off");
    }

    void UpdateVisuals()
    {
        // Show prompt only when player is near AND circle is hidden
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(playerInRange && !circulo_0.activeSelf);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw interaction radius in editor
        Gizmos.color = new Color(1, 0.5f, 0, 0.3f); // Orange with transparency
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}