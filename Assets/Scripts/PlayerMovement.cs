using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Variables
    [SerializeField] private float movSpeed;
    public float speedX;
    private float speedY;
    private Rigidbody2D rb;

    // movement data
    private Vector2 _dest = Vector2.zero;
    private float _moveDuration = 0;
    private GameObject _caller;

    public bool canMove = true;

    public static event Action<GameObject> MovementCompleted;

    public static event Action StepCompleted;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (canMove)
        {
            speedX = Input.GetAxisRaw("Horizontal") * movSpeed;
            speedY = Input.GetAxisRaw("Vertical") * movSpeed;
            rb.velocity = new Vector2(speedX, speedY);
            // for each metre of movement increment steps
            float metres = rb.velocity.magnitude;

            // check if we've moved a full metre
            if ((int)(GameManager.Instance.stepsTakenInOverworld + (metres * Time.deltaTime)) > GameManager.Instance.stepsTakenInOverworld)
            {
                StepCompleted?.Invoke();
            }
            GameManager.Instance.stepsTakenInOverworld += metres * Time.deltaTime;
        }
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

    public float GetXSpeed()
    {
        return speedX;
    }

    public float GetYSpeed()
    {
        return speedY;
    }

    public IEnumerator _MoveToLocation()
    {
        canMove = false;
        // moves .1 metres per call
        // interp by 10%
        // also make sure to set velocity to 0
        Vector2 startPos = (Vector2)transform.position; // get full movement, but only go for a fraction]
        Vector2 moveVector = (_dest - startPos) * 0.1f;
        int callNo = (int)(Vector2.Distance(startPos, _dest) / 0.1f);

        float delay = _moveDuration / callNo;

        for (int i = 0; i < callNo; i++)
        {
            transform.position += (Vector3)moveVector;
            yield return new WaitForSeconds(delay);
        }

        // snap to destination
        transform.position = _dest;
        MovementCompleted?.Invoke(_caller);
        canMove = true;
    }

    public void MoveToLocation(Vector2 dest, float moveDuration, GameObject caller)
    {
        _dest = dest;
        _moveDuration = moveDuration;
        _caller = caller;

        StartCoroutine("_MoveToLocation");
    }
}
