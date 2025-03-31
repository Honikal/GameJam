
using System.Xml.Serialization;
using UnityEngine;

public class Movement : MonoBehaviour
{

    public float moveSpeed; 
    public Rigidbody2D rigidbody;

    private Vector2 moveDirection;

    void FixedUpdate()
    {
        Move();
    }

    void Update()
    {
        ProccesInputs();
    }


    void ProccesInputs(){
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector2(moveX, moveY).normalized;
    }

    void Move()
    {
        rigidbody.linearVelocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
    }


}
