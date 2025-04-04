using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EnemyPatrol : EnemyMovement
{
    private Vector2 previousPosition;
    private float stuckTime;
    private float stuckThreshold = 0.5f;
    private void OnCollisionEnter2D(Collision2D collision)
    {
       
        if (collision.gameObject.CompareTag("Wall") && this.enabled)
        {
            Debug.Log("Wall collision");
            ChangeDirection();
        }
    }
    
    void Start()
    {
        previousPosition = transform.position;
        stuckTime = 0f;
    }

    
    void Update()
    {
        CheckIfStuck();
    }

    private void CheckIfStuck()
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

    private void ChangeDirection()
    {
        Vector2[] possibleDirections = { Vector2.up, Vector2.down, Vector2.left, Vector2.right, new Vector2(1, 1), new Vector2(1, -1), new Vector2(-1, 1), new Vector2(-1, -1) };
        Vector2 newDirection = possibleDirections[Random.Range(0, possibleDirections.Length)];
        Vector2 currentDirection = this.enemy.direction;

        while (newDirection == currentDirection)
        {
            newDirection = possibleDirections[Random.Range(0, possibleDirections.Length)];
        }

        this.enemy.SetDirection(newDirection, true);
    }

    private List<Vector2> CheckAvailableDirections()
    {
        List<Vector2> availableDirections = new();
        Vector2[] possibleDirections = { Vector2.up, Vector2.down, Vector2.left, Vector2.right, new Vector2(1, 1), new Vector2(1, -1), new Vector2(-1, 1), new Vector2(-1, -1) };
        foreach (Vector2 direction in possibleDirections)
        {
            RaycastHit2D hit = Physics2D.BoxCast(this.transform.position, Vector2.one * 0.5f, 0.0f, direction, 0.01f, this.enemy.obstacleLayer);
            if (hit.collider == null)
            {
                availableDirections.Add(direction);
            }
        }
        
        return availableDirections;
    }

    /*private void OnDisable()
    {
        this.enemy.chase.Enable();
    }*/

}
