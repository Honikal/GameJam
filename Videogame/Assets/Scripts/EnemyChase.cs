using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyChase : EnemyMovement
{
    private Vector2 previousPosition;
    private float stuckTime;
    private float stuckThreshold = 0.5f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        if (collision.gameObject.name != this.enemy.target.name && this.enabled)
        {
            ChangeDirection();
        }
    }

    void Start()
    {
        previousPosition = transform.position;
        stuckTime = 0f;
        MoveTowardsTarget();
    }


    void Update()
    {
        if(!this.enemy.MovementDisabled())
        {
            CheckIfStuck();
            MoveTowardsTarget();
        }
    }

    private void CheckIfStuck()
    {
        if (!this.enemy.MovementDisabled())
        {
            Vector2 currentPosition = transform.position;

            if (currentPosition == previousPosition)
            {
                stuckTime += Time.deltaTime;
                if (stuckTime >= stuckThreshold)
                {
                    Debug.Log("Object is stuck, changing direction");
                    ChangeDirection();
                    stuckTime = 0f;
                }
            }
            else
            {
                stuckTime = 0f;
            }

            previousPosition = currentPosition;
        }
        
    }

    private void ChangeDirection()
    {
        Vector2 direction = Vector2.zero;
        float minDistance = float.MaxValue;
        Vector2 currentDirection = this.enemy.direction;
        List<Vector2> availableDirections = CheckAvailableDirections();

        foreach (Vector2 availableDirection in availableDirections)
        {
            if (availableDirection != currentDirection)
            {
                Vector3 newPosition = this.transform.position + new Vector3(availableDirection.x, availableDirection.y, 0.0f);
                float distance = (this.enemy.target.transform.position - newPosition).sqrMagnitude;

                if (distance < minDistance)
                {
                    minDistance = distance;
                    direction = availableDirection;
                }
            }
            

        }
        if (direction != Vector2.zero)
        {
            this.enemy.SetDirection(direction, true);
        }
        else
        {
            Debug.LogWarning("No valid direction found to move.");
        }

    }

    private List<Vector2> CheckAvailableDirections()
    {
        List<Vector2> availableDirections = new();
        Vector2[] possibleDirections = { Vector2.up, Vector2.down, Vector2.left, Vector2.right, new Vector2(1, 1), new Vector2(1, -1), new Vector2(-1, 1), new Vector2(-1, -1) };
        Collider2D enemyCollider = this.GetComponent<Collider2D>();
        Collider2D targetCollider = this.enemy.target.GetComponent<Collider2D>();

        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(this.enemy.obstacleLayer);
        contactFilter.useTriggers = false;

        foreach (Vector2 direction in possibleDirections)
        {
            RaycastHit2D[] hits = new RaycastHit2D[1];
            int hitCount = Physics2D.BoxCast(this.transform.position, Vector2.one * 0.5f, 0.0f, direction, contactFilter, hits, 1.0f);

            if (hitCount == 0 || (hitCount > 0 && (hits[0].collider == enemyCollider || hits[0].collider == targetCollider)))
            {
                availableDirections.Add(direction);
            }
            else
            {
                Debug.Log("Direction " + direction + " is blocked by " + hits[0].collider.name);
            }
        }

        return availableDirections;
    }

    private void MoveTowardsTarget()
    {
        if (this.enemy.target != null && !this.enemy.MovementDisabled())
        {
            Debug.Log("Moving towards target: " + this.enemy.target.name);
            Vector2 targetPosition = this.enemy.target.transform.position;
            Vector2 currentPosition = transform.position;
            Vector2 newPosition = Vector2.MoveTowards(currentPosition, targetPosition, this.enemy.speed * Time.deltaTime);
            this.enemy.rb.MovePosition(newPosition);
        }
    }

    /*private void OnDisable()
    {
        this.enemy.patrol.Enable();
    }*/

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == this.enemy.target.name)
        {
            Debug.Log("Collision with target, disabling movement.");
            this.enemy.DisableMovement();

            Invoke("EnableMovement", 3.0f);
        }
    }
    */
    private void EnableMovement()
    {
        this.enemy.EnableMovement();
    }
}
