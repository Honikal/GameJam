using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public LayerMask obstacleLayer;
    public List<Vector2> availableDirections { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.availableDirections = new List<Vector2>();
        CheckAvailableDirection(Vector2.up);
        CheckAvailableDirection(Vector2.down);
        CheckAvailableDirection(Vector2.left);
        CheckAvailableDirection(Vector2.right);
        CheckAvailableDirection(new Vector2(1, 1));
        CheckAvailableDirection(new Vector2(1, -1));
        CheckAvailableDirection(new Vector2(-1, 1));
        CheckAvailableDirection(new Vector2(-1, -1));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CheckAvailableDirection(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(this.transform.position, Vector2.one * 0.5f, 0.0f, direction, 1.0f, this.obstacleLayer);
        if (hit.collider == null)
        {
            this.availableDirections.Add(direction);
        }
    }
}
