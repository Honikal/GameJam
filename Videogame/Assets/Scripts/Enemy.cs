using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Movement movement { get; private set; }
    public EnemyPatrol patrol { get; private set; }
    public EnemyChase chase { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public Vector2 nextDirection { get; private set; }
    public Vector2 direction { get; private set; }
    
    //public AudioClip audioClip;
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


    //Animaci�n
    private Animator enemyAnimator;
    private float movementThreshold = 0.1f;
    private Vector2 lastDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyAnimator = GetComponent<Animator>();
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
        //audio.sounds[0] = audioClip;
        chase.Disable();
        patrol.Enable();

        //Actualizamos para la animaci�n
        lastDirection = initialDirection.normalized;
        enemyAnimator.SetFloat("LstMoveX", lastDirection.x);
        enemyAnimator.SetFloat("LstMoveY", lastDirection.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (isMovementDisabled)
        {
            //El enemigo se detiene
            rb.linearVelocity = Vector2.zero;
            return;
        }
        
        if (isTargetInside)
        {
            if (!playerMovement.isEyesClosed) {
                //Debug.Log("CHASE MODE");    
                patrol.Disable();
                chase.Enable();
                EnableMovement();
            } else
            {
                DisableMovement();
                Invoke("EnableMovement", 1f);
            }
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
            //Guardamos la �ltima direcci�n antes de parar
            lastDirection = direction.normalized;
            UpdateAnimationParameters();
            return;
        }
        else
        {
            Vector2 position = rb.position;
            Vector2 translation = speed * speedMultiplier * Time.fixedDeltaTime * direction;

            rb.MovePosition(position + translation);
        }

        //Siempre actualizar los par�metros de animaci�n
        UpdateAnimationParameters();
    }

    public void SetDirection(Vector2 direction, bool forced = false)
    {
        if (forced || !Occupied(direction))
        {
            this.direction = direction;
            nextDirection = Vector2.zero;
            Debug.Log("Direction set to: " + direction);
            UpdateAnimationParameters();
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
                Debug.Log("Playing ghost sound");
                AudioManager.Instance.Play("Ghost");
                Debug.Log("Target entered the detection collider.");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (target != null && collision.gameObject.name == target.name)
        {
            isTargetInside = false;
            AudioManager.Instance.Stop("Ghost");
            Debug.Log("Target exited the detection collider.");           
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

    private void UpdateAnimationParameters()
    {
        //Primero, objtenemos la direcci�n del movimiento de velocidad

        Vector2 actualMovement = rb.linearVelocity.normalized;
        bool isActuallyMoving = rb.linearVelocity.magnitude > movementThreshold;

        //Usamos un Vector de 8 direcciones para hacer las animaciones m�s suave
        Vector2 roundedDirection = new Vector2(
            Mathf.Round(actualMovement.x),
            Mathf.Round(actualMovement.y)
        );

        //Actualizamos los �rboles de animaci�n de movimiento
        enemyAnimator.SetFloat("Horizontal", roundedDirection.x);
        enemyAnimator.SetFloat("Vertical", roundedDirection.y);

        //Actualizamos la �ltima direcci�n de movimiento

        if (isActuallyMoving)
        {
            //Nos movemos, tenemos que actualizar constantemente la �ltima direcci�n por la cual nos movimos
            lastDirection = roundedDirection;
            enemyAnimator.SetFloat("LstMoveX", roundedDirection.x);
            enemyAnimator.SetFloat("LstMoveY", roundedDirection.y);
        } else
        {
            //Mantenemos la �ltima direcci�n estando idle
            enemyAnimator.SetFloat("Horizontal", lastDirection.x);
            enemyAnimator.SetFloat("Vertical", lastDirection.y);
        }

        enemyAnimator.SetBool("isMoving", isActuallyMoving);
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
