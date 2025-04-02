using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Enemy))]
public class EnemyMovement : MonoBehaviour
{
    
    //Manejamos una serie de estados para el monstruo (Idle Movement, Chase Movement)
    enum State { Patrolling, Chase };
    private State currentState;
    public CircleCollider2D areaCheckPlayer;

    public Enemy enemy { get; private set; }

    private void Awake()
    {
        this.enemy = GetComponent<Enemy>();
        this.enabled = false;
    }

    /*public void Enable()
    {
        Enable(this.duration);
    }*/

    public virtual void Enable()
    {
        this.enabled = true;

        //CancelInvoke();
        //Invoke(nameof(Disable), 0);
    }

    public virtual void Disable()
    {
        this.enabled = false;
        CancelInvoke();
    }

    /*
    //Tenemos acceso al objeto al que el jugador intentar� perseguir
    [Header("Sistema de Movimiento")]
    public float moveSpeed;                     //Manejamos la velocidad del monstruo 
    public float chaseRange = 5f;               //Rango en el que el enemigo empezar� a perseguir al jugador
    public float waypointReachDistance = 0.5f;  //Distancia entre waypoint a alcanzar
    public float obstacleCheckDistance = 1f;    //Distancia para checar la posibilidad o existencia de un obst�culo

    public float detectionRadius = 10f;         //Radio para detectar al jugador
    public float pathRecalculationTime = 2f;    //Tiempo para volver a recalcular el tiempo de movimiento

    [Header("Sistema de Waypoints")]
    public Transform[] waypoints;           //Puntos en los que el enemigo decide detenerse para hacer patrulla

    private Transform target;               //Extraemos el jugador u objeto a perseguir
    private int currentWaypointIndex;       //Manejamos el punto o ubicaci�n actual del enemigo de acuerdo al waypoint  
    private Rigidbody2D rb;                 //Manejamos el movimiento o direcci�n del objeto
    private Vector2 currentDirection;       //Manejamos la direccion actual
    private LayerMask obstacleMask;         //Capa que contiene los obst�culos a considerar
    private float pathTimer;                //Timer encargado de cargar cada vez que se requiere recalcular movimiento

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Tomamos las referencias a usar
        rb = GetComponent<Rigidbody2D>(); //Tomamos el cuerpo r�gido del objeto enemigo
        target = GameObject.FindGameObjectWithTag("Player").transform;
        obstacleMask = LayerMask.GetMask("Obstacles", "Collision");
        currentWaypointIndex = 0; //M�todo anterior siguiendo los mismos puntos a seguir
        currentState = State.Patrolling;
        //SelectNewRandomWaypoint();   M�todo nuevo a usar
    }

    //Manejamos el sistema de estados del enemigo
    void Update()
    {
        switch (currentState)
        {
            case State.Patrolling:
                PatrolBehavior();       //Llamamos a la funci�n para determinar que haga un sistema de patrulla
                break;
            case State.Chase:
                ChaseBehavior();        //Llamamos a la funci�n para que persiga al jugador
                break;
        }
    }

    //Manejamos el movimiento de Patrolling del monstruo

    /*
    private void PatrolBehavior()
    {
        //Ac� manejamos tambi�n el sistema de waypoints
        if (waypoints.Length == 0) return;          //Si no hay waypoints, no hay punto donde el monstruo avance

        //Tomamos la direcci�n normalizada donde el enemigo se mover�
        Vector2 desiredDirection = (waypoints[currentWaypointIndex].position - transform.position).normalized;
        currentDirection = GetObstacleAvoidanceDirection(desiredDirection);

        //Aplicamos el movimiento deseado
        rb.linearVelocity = desiredDirection * moveSpeed;

        //Checamos si se est� atrapado o requiere recalculaci�n del camino
        if (pathTimer >= pathRecalculationTime ||
            Vector2.Distance(transform.position, waypoints[currentWaypointIndex].position) < waypointReachDistance)
        {
            SelectNewRandomWaypoint();
            pathTimer = 0f;
        }

    }
    


    private void PatrolBehavior()
    {
        // Ac� manejamos tambi�n el sistema de waypoints
        if (waypoints.Length == 0) return;          //Si no hay waypoints, no hay punto donde el monstruo avance

        //Tomamos la direcci�n normalizada donde el enemigo se mover�
        Vector2 direction = (waypoints[currentWaypointIndex].position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed; //* Time.deltatime;

        //Checamos para que se mueva al siguiente punto sin tener problemas
        if (Vector2.Distance(transform.position, waypoints[currentWaypointIndex].position) < waypointReachDistance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    /*
    private Vector2 GetObstacleAvoidanceDirection(Vector2 desiredDirection)
    {
        //Ac� intentamos conseguir en base a una direcci�n inicial, si hay un obst�culo en medio, el vector deseado para
        //llegar al punto deseado, ac� usamos RayCasting

        //Casteamos rayos en distintas ubicaciones
        float[] angles = { 0f, 30f, -30f, 45f, -45f };
        float maxDistance = 0f;
        Vector2 bestDirection = desiredDirection;

        foreach (float angle in angles)
        {
            Vector2 dir = Quaternion.Euler(0, 0, angle) * desiredDirection;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, obstacleCheckDistance, obstacleMask);

            if (!hit.collider && dir.magnitude > maxDistance)
            {
                //Si no se llega a encontrar un punto a chocar y la distancia es mayor a la distancia m�xima, entonces
                //asignamos dicha distancia como la mejor
                maxDistance = dir.magnitude;
                bestDirection = dir;
            }
        }

        return bestDirection.normalized;
    }
    */

    /*
    private void SelectNewRandomWaypoint()
    {
        //Checamos en el caso que hayan m�s de 2 puntos
        if (waypoints.Length < 2) return;

        //Buscamos el waypoint m�s cercano una vez se est� bloqueado
        float closestDistance = Mathf.Infinity;
        int newIndex = currentWaypointIndex;

        for (int i = 0; i < waypoints.Length; i++)
        {
            if (i == currentWaypointIndex) continue;

            float distance = Vector2.Distance(transform.position, waypoints[i].position);
            
            //Comparamos distancias, as� hasta llegar a encontrar la de menor costo
            if (distance < closestDistance)
            {
                closestDistance = distance;
                newIndex = i;
            }
        }

        currentWaypointIndex = newIndex;
    }
    


    private void ChaseBehavior()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
    }



    /*
    //Ayuda visual para checar movimiento
    private void OnDrawGizmos()
    {
        //Dibujamos el radio de detecci�n del monstruo
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius); 

        //Dibujamos el path actual
        if (waypoints != null && currentWaypointIndex < waypoints.Length && waypoints[currentWaypointIndex] != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, waypoints[currentWaypointIndex].position);
        }

        //Dibujamos el Raycasting
        Gizmos.color = Color.red;
        Vector2 desiredDirection = (waypoints.Length > 0 && currentWaypointIndex < waypoints.Length) ?
            (waypoints[currentWaypointIndex].position - transform.position).normalized :
            Vector2.zero;
        float[] angles = { 0f, 30f, -30f, 45f, -45f };
        foreach (float angle in angles)
        {
            Vector2 dir = Quaternion.Euler(0, 0, angle) * desiredDirection;
            Gizmos.DrawRay(transform.position, dir * obstacleCheckDistance);
        }

        //Pintamos o dise�amos el camino a seguir por waypoints
        if (waypoints != null && waypoints.Length > 1)
        {
            Gizmos.color = Color.cyan;
            foreach (Transform wp in waypoints)
            {
                if (wp != null) Gizmos.DrawSphere(wp.position, 0.3f);
            }
        }
    }
    


    private void OnDrawGizmos()
    {
        //Dibujamos el radio de detecci�n del monstruo
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        //Pintamos o dise�amos el camino a seguir por waypoints
        if (waypoints != null && waypoints.Length > 1)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < waypoints.Length; i++)
            {
                //Si llegamos al fin o a un punto donde hay un waypoint nulo, continuamos
                if (waypoints[i] == null) continue;

                //Dibujamos una pequela esfera para simbolizar el waypoint
                Gizmos.DrawSphere(waypoints[i].position, 0.2f);

                if (i < waypoints.Length - 1)
                {
                    //Dibujamos una l�nea entre el punto actual y el siguiente waypoint
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                }
                else
                {
                    //Dibujamos en �ste punto, una l�nea entre el punto final y el punto inicial
                    Gizmos.DrawLine(waypoints[i].position, waypoints[0].position);
                }
            }
        }
    }
    */
}
