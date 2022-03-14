using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Utility.Addition;

public class SquareController : MonoBehaviour
{
    [SerializeField] private GameObject  parent;
    [SerializeField] private Vector2Int  positionInBoard;

    private BoardMovement  boardMovement;
    private SpriteRenderer sprite;

    private Vector2 startMousePosition;
    private bool mouseDowned;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        boardMovement = parent.GetComponent<BoardMovement>();
    }

    private void Start()
    {
        mouseDowned = false;
    }

    // Setters

    public void setPositionInBoard(Vector2Int positionInBoard)
    {
        this.positionInBoard = positionInBoard;
    }

    public void setColor(Color color)
    {
        sprite.color = color;
    }
    public void setParent(GameObject parent)
    {
        this.parent = parent;
    }

    // Colllisions

    private void OnMouseDown()
    {
        startMousePosition = Input.mousePosition;
        mouseDowned = true;
    }

    private void OnMouseUp()
    {
        mouseDowned = false;
    }
    
    private void OnMouseExit()
    {
        if (!mouseDowned || boardMovement.inMoving)
            return;

        mouseDowned = false;

        Vector2 endMousePosition = Input.mousePosition;
        Vector2 delta = endMousePosition - startMousePosition;

        Direction direction;

        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            direction = delta.x > 0 ? Direction.right : Direction.left;
        else
            direction = delta.y > 0 ? Direction.down : Direction.up;

        boardMovement.Replacement(positionInBoard, direction);
    }
    
}
