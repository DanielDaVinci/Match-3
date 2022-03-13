using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareController : MonoBehaviour
{
    [SerializeField] private GameObject  board;
    [SerializeField] private Vector2Int  positionInBoard;

    private BoardManager   boardManager;
    private SpriteRenderer sprite;

    private Vector2 startMousePosition;
    private bool mouseDowned;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        boardManager = board.GetComponent<BoardManager>();
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
    public void setBoard(GameObject board)
    {
        this.board = board;
    }

    // Colllisions

    private void OnMouseDown()
    {
        startMousePosition = Input.mousePosition;
        mouseDowned = true;
    }

    private void OnMouseUpAsButton()
    {
        mouseDowned = false;
    }

    private void OnMouseExit()
    {
        if (!mouseDowned || BoardManager.inMoving)
            return;

        mouseDowned = false;

        Vector2 endMousePosition = Input.mousePosition;
        Vector2 delta = endMousePosition - startMousePosition;

        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            if (delta.x > 0)
                boardManager.Replacement(positionInBoard, BoardManager.Direction.right);
            else
                boardManager.Replacement(positionInBoard, BoardManager.Direction.left);
        }
        else
        {
            if (delta.y > 0)
                boardManager.Replacement(positionInBoard, BoardManager.Direction.down);
            else
                boardManager.Replacement(positionInBoard, BoardManager.Direction.up);
        }
    }
}
