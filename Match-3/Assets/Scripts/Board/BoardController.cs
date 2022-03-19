using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Board))]
public class BoardController : MonoBehaviour
{
    private Board board;

    private void Awake()
    {
        board = GetComponent<Board>();
    }

    private void Start()
    {
        ShiftPositionToCenture();
        MakeBoard();
        SetColors();
    }

    private void ShiftPositionToCenture()
    {
        Vector2 size = board.Prefab.GetComponent<SpriteRenderer>().size * board.Prefab.transform.localScale;

        bool evenX = board.SizeX % 2 == 0;
        bool evenY = board.SizeY % 2 == 0;
        float deltaX = (size.x + board.Distance) * (board.SizeX / 2) + (evenX ? -board.Distance : size.x) / 2;
        float deltaY = (size.y + board.Distance) * (board.SizeY / 2) + (evenY ? -board.Distance : size.y) / 2;

        transform.position -= new Vector3(deltaX, deltaY, 0);
    }

    private void MakeBoard()
    {
        board.Prefab.GetComponent<SquareController>().setParent(gameObject);
        Vector2 size = board.Prefab.GetComponent<SpriteRenderer>().size * board.Prefab.transform.localScale;

        for (int i = 0; i < board.SizeX; i++)
        {
            for (int j = 0; j < board.SizeY; j++)
            {
                Vector2 position = GetRelativePosition(new Vector2Int(i, j), size);
                board[i, j] = Instantiate(board.Prefab);
                board[i, j].transform.SetParent(transform, false);
                board[i, j].transform.localPosition = position;
                SquareController sqController = board[i, j].GetComponent<SquareController>();
                sqController.setPositionInBoard(new Vector2Int(i, j));
            }
        }
    }

    public Vector2 GetRelativePosition(Vector2Int postionInBoard, Vector2 size)
    {
        float posX = postionInBoard.x * (size.x + board.Distance) + size.x / 2;
        float posY = postionInBoard.y * (size.y + board.Distance) + size.y / 2;

        return new Vector2(posX, posY);
    }

    public Vector2 GetRelativePosition(Vector2Int postionInBoard)
    {
        Vector2 size = board.Prefab.GetComponent<SpriteRenderer>().size * board.Prefab.transform.localScale;

        float posX = postionInBoard.x * (size.x + board.Distance) + size.x / 2;
        float posY = postionInBoard.y * (size.y + board.Distance) + size.y / 2;

        return new Vector2(posX, posY);
    }

    public void SetColors()
    {
        if (board.Colors.Count == 0)
            return;

        for (int i = 0; i < board.SizeX; i++)
        {
            for (int j = 0; j < board.SizeY; j++)
            {
                List<Color> updateColors = new List<Color>(board.Colors);

                if (i > 1)
                {
                    Color color1 = board[i - 1, j].GetComponent<SpriteRenderer>().color;
                    Color color2 = board[i - 2, j].GetComponent<SpriteRenderer>().color;
                    if (color1 == color2)
                        updateColors.Remove(color1);
                }
                if (j > 1)
                {
                    Color color1 = board[i, j - 1].GetComponent<SpriteRenderer>().color;
                    Color color2 = board[i, j - 2].GetComponent<SpriteRenderer>().color;
                    if (color1 == color2)
                        updateColors.Remove(color1);
                }

                Color randomColor = updateColors[Random.Range(0, updateColors.Count)];
                board[i, j].GetComponent<SquareController>().setColor(randomColor);
            }
        }
    }
}
