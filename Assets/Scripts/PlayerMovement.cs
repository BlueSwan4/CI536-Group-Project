using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Variables
    [SerializeField] private float movSpeed;
    private float speedX;
    private float speedY;
    private Rigidbody2D rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        speedX = Input.GetAxisRaw("Horizontal") * movSpeed;
        speedY = Input.GetAxisRaw("Vertical") * movSpeed;
        rb.velocity = new Vector2(speedX, speedY);
        // for each metre of movement increment steps
        float metres = rb.velocity.magnitude;
        GameManager.Instance.stepsTakenInOverworld += metres * Time.deltaTime;
    }

    public void DisableMovement()
    {
        rb.velocity = new Vector2(0, 0);
        enabled = false;
    }

    public void EnableMovement()
    {
        enabled = true;
    }
}
