using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Movement movement { get; private set; }
    public EnemyPatrol patrol { get; private set; }
    public Vector2 initialPosition;
    public Transform target;
    public Vector2 initialDirection;
    public LayerMask obstacleLayer;
    public Rigidbody2D rb { get; private set; }
    public Vector2 nextDirection { get; private set; }
    public float speed = 8f;
    public float speedMultiplier = 1f;
    public Vector2 direction { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        this.movement = this.GetComponent<Movement>();
        this.patrol = this.GetComponent<EnemyPatrol>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = initialPosition;
        direction = initialDirection;
        patrol.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (nextDirection != Vector2.zero)
        {
            SetDirection(nextDirection);
        }
    }
    private void FixedUpdate()
    {
        Vector2 position = rb.position;
        Vector2 translation = speed * speedMultiplier * Time.fixedDeltaTime * direction;

        rb.MovePosition(position + translation);
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
