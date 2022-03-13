using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareMovement : MonoBehaviour
{
    [SerializeField] private float speed;

    private Vector2 direction;
    private Vector2 endPosition;

    public bool isMoving;

    void Update()
    {
        if (isMoving)
        {
            transform.Translate(direction * speed * Time.deltaTime);

            Vector2 delta = endPosition - (Vector2)transform.position;

            if ((Mathf.Abs(delta.x) <= 0.001f && direction.y == 0) || (Mathf.Abs(delta.y) <= 0.001f && direction.x == 0))
            {
                transform.position = endPosition;
                isMoving = false;
            }
        }
    }

    public void MoveTo(Vector2 endPosition)
    {
        this.endPosition = endPosition;
        direction = (endPosition - (Vector2)transform.position).normalized;
        Debug.Log(direction);

        isMoving = true;
    }
}
