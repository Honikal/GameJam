using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] float healthAmount = 20f; // Amount of health to restore
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Try to get the Movement component from the player
            Movement playerMovement = collision.GetComponent<Movement>();
            
            if (playerMovement != null)
            {
                Debug.Log("Se encontró al jugador, añadiendo vida:");

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