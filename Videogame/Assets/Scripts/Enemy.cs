using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Movement movement { get; private set; }
    public EnemyPatrol patrol { get; private set; }
    public EnemyChase chase { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public Vector2 nextDirection { get; private set; }
    public Vector2 direction { get; private set; }
    public GameObject target;

    public Vector2 initialPosition;
    //public Transform player;
    public Vector2 initialDirection;
    public LayerMask obstacleLayer;
    public CircleCollider2D detectionCollider;

    public float speed = 4f;
    public float speedMultiplier = 1f;
    private Movement playerMovement;
    private bool isTargetInside = false;
    private bool isMovementDisabled = false;
    


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        this.movement = this.GetComponent<Movement>();
        this.patrol = this.GetComponent<EnemyPatrol>();
        this.chase = this.GetComponent<EnemyChase>();
        detectionCollider = GetComponent<CircleCollider2D>();

        if (target != null)
        {
            playerMovement = target.GetComponent<Movement>();
            if (playerMovement == null)
            {
                Debug.LogError("Movement component not found on target.");
            }
        }
        else { Debug.LogError("Target not assigned."); }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = initialPosition;
        direction = initialDirection;
        chase.Disable();
        patrol.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (isMovementDisabled)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        
        if (isTargetInside)
        {
            //if (!playerMovement.isEyesClosed)
            //{
            //Debug.Log("CHASE MODE");    
            patrol.Disable();
            chase.Enable();
            EnableMovement();
            /*}
            else
            {
                DisableMovement();
            }*/
        }
        else
        {
            //Debug.Log("PATROL MODE");
            chase.Disable();
            patrol.Enable();
            EnableMovement();     
        }
        if (nextDirection != Vector2.zero && !isMovementDisabled)
        {
            SetDirection(nextDirection);
        }
    }
    private void FixedUpdate()
    {
        if (isMovementDisabled)
        {
            rb.linearVelocity = Vector2.zero;
            
        }
        else
        {
            Vector2 position = rb.position;
            Vector2 translation = speed * speedMultiplier * Time.fixedDeltaTime * direction;

            rb.MovePosition(position + translation);
        }
    }

    public void SetDirection(Vector2 direction, bool forced = false)
    {
        if (forced || !Occupied(direction))
        {
            this.direction = direction;
            nextDirection = Vector2.zero;
            Debug.Log("Direction set to: " + direction);

        }
        else
        {
            nextDirection = direction;
        }
        Debug.Log("Next direction set to: " + nextDirection);
    }

    public bool Occupied(Vector2 direction)
    {
        // If no collider is hit then there is no obstacle in that direction
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one * 0.75f, 0f, direction, 1.0f, obstacleLayer);
        return hit.collider != null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger entered: " + collision.gameObject + target);
        if (target != null)
        {
            if (collision.gameObject.name == target.name)
            {
                isTargetInside = true;
                Debug.Log("Target entered the detection collider.");
            }
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (target != null)
        {
            if (collision.gameObject.name == target.name)
            {
                isTargetInside = false;
                Debug.Log("Target exited the detection collider.");
            }
        }
        
    }

    public void DisableMovement()
    {
        //Debug.Log("Movement disabled");
        isMovementDisabled = true;
        rb.linearVelocity = Vector2.zero; 
        rb.bodyType = RigidbodyType2D.Kinematic; 
    }

    public void EnableMovement()
    {
        //Debug.Log("Movement enabled");
        isMovementDisabled = false;
        rb.bodyType = RigidbodyType2D.Dynamic; 
    }

    public bool MovementDisabled()
    {
        return isMovementDisabled;
    }

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Vector2[] possibleDirections = { Vector2.up, Vector2.down, Vector2.left, Vector2.right, new Vector2(1, 1), new Vector2(1,-1), new Vector2(-1,1), new Vector2(-1,-1) };
            Vector2 newDirection = possibleDirections[Random.Range(0, possibleDirections.Length)];

            while (newDirection == direction)
            {
                newDirection = possibleDirections[Random.Range(0, possibleDirections.Length)];
            }

            SetDirection(newDirection, true);
        }
    }

    private Vector2 CheckAvailableDirections()
    {
        Vector2[] possibleDirections = { Vector2.up, Vector2.down, Vector2.left, Vector2.right, new Vector2(1, 1), new Vector2(1, -1), new Vector2(-1, 1), new Vector2(-1, -1) };
        Vector2 newDirection = possibleDirections[Random.Range(0, possibleDirections.Length)];

        while (newDirection == direction)
        {
            newDirection = possibleDirections[Random.Range(0, possibleDirections.Length)];
        }

        return newDirection;
    }*/
}
