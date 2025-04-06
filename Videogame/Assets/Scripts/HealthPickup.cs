using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] float healthAmount = 20f; // Amount of health to restore
    public AudioManager audioManager { get; private set; }

    private void Awake()
    {
        audioManager = AudioManager.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Try to get the Movement component from the player
            Movement playerMovement = collision.GetComponent<Movement>();
            
            if (playerMovement != null)
            {
                Debug.Log("Se encontr� al jugador, a�adiendo vida:");
                audioManager.Play("healthpickup");
                // Restore health (you'll need to add a public method in Movement)
                playerMovement.IncreaseHealth(healthAmount);

                // Play sound if available
                //AudioManager.Instance.Play("HealthPickup");


                Debug.Log("Destruyendo pickup");
                // Destroy the pickup
                Destroy(gameObject);
            }
        }
    }
}