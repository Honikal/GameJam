using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] float healthAmount = 20f; // Amount of health to restore
    [SerializeField] AudioClip pickupSound; // Optional sound effect
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Try to get the Movement component from the player
            Movement playerMovement = collision.GetComponent<Movement>();
            
            if (playerMovement != null)
            {
                // Restore health (you'll need to add a public method in Movement)
                playerMovement.IncreaseHealth(healthAmount);
                
                // Play sound if available
                if (pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                }
                
                // Destroy the pickup
                Destroy(gameObject);
            }
        }
    }
}