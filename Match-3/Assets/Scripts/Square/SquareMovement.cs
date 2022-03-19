using UnityEngine;

public class SquareMovement : MonoBehaviour
{
    [SerializeField] private float speed = 1;
    [SerializeField] private float epsilon = 0.01f;

    private Vector2 direction;
    private Vector2 endPosition;

    [HideInInspector] public bool isMoving;

    void Update()
    {
        if (isMoving)
        {
            transform.Translate(direction * speed * Time.deltaTime);
            Vector2 delta = endPosition - (Vector2)transform.localPosition;

            if ((Mathf.Abs(delta.x) <= epsilon && direction.y == 0) || (Mathf.Abs(delta.y) <= epsilon && direction.x == 0))
            {
                transform.localPosition = endPosition;
                isMoving = false;
            }
        }
    }

    public void MoveTo(Vector2 endPosition)
    {
        this.endPosition = endPosition;
        direction = (endPosition - (Vector2)transform.localPosition).normalized;

        isMoving = true;
    }
}
