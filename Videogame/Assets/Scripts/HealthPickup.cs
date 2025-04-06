using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] float healthAmount = 20f; // Amount of health to restore
    [SerializeField] LayerMask playerLayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.CompareTag("Player") && collision.CompareTag("Light"))
        //Checaremos capa en vez de tag
        if (((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            // Try to get the Movement component from the player
            Movement playerMovement = collision.GetComponent<Movement>();
            
            if (playerMovement != null)
            {
                Debug.Log("Se encontro al jugador, agregando vida:");
                AudioManager.Instance.Play("HealthPickup");
                // Restore health (you'll need to add a public method in Movement)
                playerMovement.IncreaseHealth(healthAmount);

                Debug.Log("Destruyendo pickup");
                // Destroy the pickup
                Destroy(gameObject);
            }
        }
    }
}